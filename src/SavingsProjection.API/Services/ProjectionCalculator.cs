using Microsoft.EntityFrameworkCore;
using SavingsProjection.API.Infrastructure;
using SavingsProjection.API.Services.Abstract;
using SavingsProjection.Model;
using System.Security.Cryptography.Xml;

namespace SavingsProjection.API.Services
{
    public class ProjectionCalculator : IProjectionCalculator
    {
        private readonly SavingProjectionContext context;

        public ProjectionCalculator(SavingProjectionContext context)
        {
            this.context = context;
        }

        public async Task SaveProjectionToHistory()
        {
            var projectionItems = await CalculateAsync(null, null, true, false, false);
            await this.context.MaterializedMoneyItems.AddRangeAsync(projectionItems);
            await this.context.SaveChangesAsync();
        }

        private decimal CalculateCash(List<FixedMoneyItem> itemsToAccumulate, List<FixedMoneyItem> itemsNotAccumulate, Configuration config, decimal additionalCashLeft,DateTime periodStart)
        {
            var cashItems = itemsNotAccumulate
                            .Where(x => x.CategoryID != config.CashWithdrawalCategoryID && x.Cash)
                            .Union(itemsToAccumulate
                            .Where(x => x.CategoryID != config.CashWithdrawalCategoryID && x.Cash))
                            .ToList();


            var cashWithdrawal = itemsNotAccumulate
                .Where(x => x.CategoryID == config.CashWithdrawalCategoryID)
                .Union(itemsToAccumulate
                .Where(x => x.CategoryID == config.CashWithdrawalCategoryID))
                .OrderBy(x => x.Date).ToList();

            if (additionalCashLeft != 0)
            {
                cashWithdrawal.Insert(0, new FixedMoneyItem { Cash = true, Amount = additionalCashLeft, Note = "Additional Cash", Date = periodStart });
            }


            decimal carryCashExpenses = 0;
            decimal cashLeftToSpend = 0;
            foreach (var cashWithdrawalItem in cashWithdrawal)
            {
                var currentCashItems = cashItems.Where(x => x.Date >= cashWithdrawalItem.Date).OrderBy(x => x.Date);
                if (currentCashItems.Any()) cashWithdrawalItem.Note += $"(Original amount {cashWithdrawalItem.Amount})";
                if (carryCashExpenses > 0)
                {
                    cashWithdrawalItem.Amount += carryCashExpenses;
                    carryCashExpenses = 0;
                }
                var lstItemsToRemove = new List<FixedMoneyItem>();
                foreach (var currentCashItem in currentCashItems)
                {
                    cashWithdrawalItem.Amount -= currentCashItem.Amount;
                    lstItemsToRemove.Add(currentCashItem);
                    if (cashWithdrawalItem.Amount >= 0)
                    {
                        carryCashExpenses = cashWithdrawalItem.Amount.Value;
                        cashWithdrawalItem.Amount = 0;
                        break;
                    }
                }
                foreach (var item in lstItemsToRemove)
                {
                    cashItems.Remove(item);
                }
                if (cashWithdrawalItem.Amount < 0)
                {
                    cashLeftToSpend += cashWithdrawalItem.Amount.Value;
                }
            }
            return cashLeftToSpend;
        }

        public async Task<IEnumerable<MaterializedMoneyItem>> CalculateAsync(DateTime? from, DateTime? to, bool breakFirstEndPeriod = false, bool onlyInstallment = false, bool includeLastEndPeriod = true)
        {
            var res = new List<MaterializedMoneyItem>();
            var lastEndPeriod = context.MaterializedMoneyItems.Where(x => x.EndPeriod).OrderByDescending(x => x.Date).FirstOrDefault();
            if (lastEndPeriod != null && includeLastEndPeriod) res.Add(lastEndPeriod);
            var fromDate = lastEndPeriod?.Date ?? throw new Exception("Unable to define the starting time");
            var periodStart = fromDate.AddDays(1);
            var config = context.Configuration.FirstOrDefault() ?? throw new Exception("Unable to find the configuration");
            DateTime periodEnd;
            bool endPeriodCashCarryUsed = false;
            while ((periodEnd = CalculateNextReccurrency(periodStart, config.EndPeriodRecurrencyType, config.EndPeriodRecurrencyInterval).AddDays(-1)) <= (to ?? new DateTime(9999, 12, 31)))
            {
                if (!to.HasValue) breakFirstEndPeriod = true;
                int accumulatorStartingIndex = res.Count;
                var fixedItemsNotAccumulate = await context.FixedMoneyItems.Include(x => x.Category).Where(x => x.Date >= periodStart && x.Date <= periodEnd && !x.AccumulateForBudget).ToListAsync();
                var fixedItemsAccumulate = await context.FixedMoneyItems.Where(x => x.Date >= periodStart && x.Date <= periodEnd && x.AccumulateForBudget).ToListAsync();
                var recurrentItems = await context.RecurrentMoneyItems.Include(x => x.Adjustements).Include(x => x.AssociatedItems).Include(x => x.Category).Where(x => x.StartDate <= periodEnd && (!x.EndDate.HasValue || periodStart <= x.EndDate) && x.RecurrentMoneyItemID == null).ToListAsync();

                if (onlyInstallment) recurrentItems = recurrentItems.Where(x => x.Type == MoneyType.InstallmentPayment).ToList();

                decimal additionalCash = 0;
                if (!endPeriodCashCarryUsed)
                {
                    endPeriodCashCarryUsed = true;
                    additionalCash = lastEndPeriod.EndPeriodCashCarry;
                }
                var cashLeftToSpend = CalculateCash(fixedItemsAccumulate, fixedItemsNotAccumulate, config, additionalCash,periodStart);

                foreach (var fixedItem in fixedItemsNotAccumulate)
                {
                    res.Add(new MaterializedMoneyItem
                    {
                        Date = fixedItem.Date,
                        Category = fixedItem.Category,
                        Amount = fixedItem.Amount ?? 0,
                        EndPeriod = false,
                        Note = fixedItem.Note,
                        Type = MoneyType.Others,
                        TimelineWeight = fixedItem.TimelineWeight,
                        IsRecurrent = false,
                        FixedMoneyItemID = fixedItem.ID,
                        Cash = fixedItem.Cash
                    });
                }

                //Fixed items to accumulate for budget
                var accumulateMaterializedItem = new MaterializedMoneyItem { Date = periodStart, Note = "Accumulator", TimelineWeight = 5, IsRecurrent = false };
                foreach (var accumulateItem in fixedItemsAccumulate)
                {
                    accumulateMaterializedItem.Category = null;
                    accumulateMaterializedItem.Amount += accumulateItem.Amount ?? 0;
                    accumulateMaterializedItem.EndPeriod = false;
                    accumulateMaterializedItem.Type = MoneyType.Others;
                }
                if (fixedItemsAccumulate.Count > 0)
                {
                    accumulateMaterializedItem.Note += ": " + string.Join(',', fixedItemsAccumulate.Select(x => x.Note).ToArray());
                }
                res.Add(accumulateMaterializedItem);

                var accumulatedForBudgetLeft = accumulateMaterializedItem.Amount;
                foreach (var recurrentItem in recurrentItems)
                {
                    var installments = CalculateInstallmentInPeriod(recurrentItem, periodStart, periodEnd);

                    foreach (var installment in installments)
                    {
                        //Check the adjustements
                        var currentAdjustment = recurrentItem.Adjustements?.Where(x => x.RecurrencyDate == installment.currentDate || x.RecurrencyNewDate.HasValue && x.RecurrencyNewDate == installment.currentDate).FirstOrDefault();
                        var currentInstallmentDate = currentAdjustment?.RecurrencyNewDate ?? installment.original;
                        var currentInstallmentAmount = currentAdjustment?.RecurrencyNewAmount ?? recurrentItem.Amount;
                        var currentInstallmentNote = recurrentItem.Note;

                        //Check if is a budget for subtract the accumulated
                        if (recurrentItem.Type == MoneyType.PeriodicBudget)
                        {
                            decimal accumulatorToSubtract = accumulatedForBudgetLeft < currentInstallmentAmount ? currentInstallmentAmount : accumulatedForBudgetLeft;
                            accumulatedForBudgetLeft -= accumulatorToSubtract;
                            currentInstallmentAmount -= accumulatorToSubtract;
                        }

                        //Check if there are child items
                        if (recurrentItem.AssociatedItems != null)
                        {
                            var lstNoteAssociatedItems = new List<string>();
                            var associatedItemsToCalculate = recurrentItem.AssociatedItems.Where(x => x.StartDate <= periodEnd && (!x.EndDate.HasValue || periodStart <= x.EndDate.Value));
                            if (onlyInstallment) associatedItemsToCalculate = associatedItemsToCalculate.Where(x => x.Type == MoneyType.InstallmentPayment);
                            foreach (var associatedItem in associatedItemsToCalculate)
                            {
                                var associatedIteminstallment = CalculateInstallmentInPeriod(associatedItem, installment.original, installment.original);
                                if (associatedIteminstallment.Count() > 0)
                                {
                                    if (currentAdjustment?.RecurrencyNewAmount == null)
                                    {
                                        currentInstallmentAmount += associatedItem.Amount;
                                    }
                                    lstNoteAssociatedItems.Add(associatedItem.Note);
                                }
                            }
                            if (lstNoteAssociatedItems.Count > 0)
                            {
                                currentInstallmentNote += ": " + string.Join(',', lstNoteAssociatedItems.ToArray());
                            }

                        }

                        res.Add(new MaterializedMoneyItem
                        {
                            Date = currentInstallmentDate,
                            Category = recurrentItem.Category,
                            Amount = currentInstallmentAmount,
                            EndPeriod = false,
                            Note = currentInstallmentNote,
                            Type = recurrentItem.Type,
                            TimelineWeight = recurrentItem.TimelineWeight,
                            IsRecurrent = true,
                            RecurrentMoneyItemID = recurrentItem.ID
                        });
                    }
                }

                res.Add(new MaterializedMoneyItem { Amount = res.GetRange(accumulatorStartingIndex, res.Count - accumulatorStartingIndex).Sum(x => x.Amount), Note = $"💵 Cash: {cashLeftToSpend:N2}", Date = periodEnd, EndPeriod = true, IsRecurrent = false, EndPeriodCashCarry = cashLeftToSpend });
                periodStart = periodEnd.AddDays(1);
                if (breakFirstEndPeriod) break;
            }
            //Calculate the projection
            var lastProjectionValue = context.MaterializedMoneyItems.Where(x => x.Date <= fromDate).OrderByDescending(x => x.Date).FirstOrDefault().Projection;
            res = res.OrderBy(x => x.Date).ThenByDescending(x => x.TimelineWeight).ToList();
            foreach (var resItem in res)
            {
                resItem.Projection = lastProjectionValue + (resItem.EndPeriod ? 0 : resItem.Amount);
                lastProjectionValue = resItem.Projection;
            }
            if (from.HasValue) res.RemoveAll(x => x.Date <= from);
            return res;
        }

        IEnumerable<(DateTime original, DateTime currentDate)> CalculateInstallmentInPeriod(RecurrentMoneyItem item, DateTime periodStart, DateTime periodEnd)
        {
            var lstInstallmentsDate = new List<(DateTime original, DateTime currentDate)>();
            if (item.StartDate <= periodEnd && (!item.EndDate.HasValue || periodStart <= item.EndDate.Value))
            {
                var currentInstallmentOriginal = item.StartDate;
                var currentInstallmentDate = CalculateActualInstallmentDate(item, currentInstallmentOriginal);
                while (currentInstallmentDate <= periodEnd)
                {
                    if (currentInstallmentDate >= periodStart) lstInstallmentsDate.Add((currentInstallmentOriginal, currentInstallmentDate));
                    if (item.RecurrencyInterval == 0) break;
                    currentInstallmentOriginal = CalculateNextReccurrency(currentInstallmentOriginal, item.RecurrencyType, item.RecurrencyInterval);
                    currentInstallmentDate = CalculateActualInstallmentDate(item, currentInstallmentOriginal);
                }
            }
            return lstInstallmentsDate;
        }

        private static DateTime CalculateActualInstallmentDate(RecurrentMoneyItem item, DateTime currentInstallmentOriginal)
        {
            DateTime currentInstallmentDate;
            var adjustment = item.Adjustements?.Where(x => x.RecurrencyDate == currentInstallmentOriginal && x.RecurrencyNewDate.HasValue).FirstOrDefault();
            if (adjustment != null)
                currentInstallmentDate = adjustment.RecurrencyNewDate.Value;
            else
                currentInstallmentDate = currentInstallmentOriginal;
            return currentInstallmentDate;
        }

        DateTime CalculateNextReccurrency(DateTime currentEndPeriod, RecurrencyType recurrType, int recurrIterval)
        {
            DateTime nextEndPeriod = currentEndPeriod;
            switch (recurrType)
            {
                case RecurrencyType.Day:
                    nextEndPeriod = currentEndPeriod.AddDays(recurrIterval);
                    break;
                case RecurrencyType.Week:
                    nextEndPeriod = currentEndPeriod.AddDays(recurrIterval * 7);
                    break;
                case RecurrencyType.Month:
                    nextEndPeriod = currentEndPeriod.AddMonths(recurrIterval);
                    break;
            }
            return nextEndPeriod;
        }
    }
}
