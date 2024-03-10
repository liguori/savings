using Microsoft.AspNetCore.Components;
using Radzen;
using Savings.Model;
using Savings.SPA.Services;
using System.Linq.Dynamic.Core;

namespace Savings.SPA.Pages
{
    public partial class Reports : ComponentBase
    {

        [Inject]
        private ISavingsApi savingsAPI { get; set; }

        [Inject]
        public DialogService dialogService { get; set; }

        private RecurrentMoneyItem[] RecurrentItems { get; set; }

        private MaterializedMoneyItem[] Installments { get; set; }

        private MaterializedMoneyItem[] EndPeriods { get; set; }

        public string FilterCategoryGroupByPeriod { get; set; } = "yy/MM";

        public DateTime FilterDateFrom { get; set; }

        public DateTime FilterDateTo { get; set; }

        ReportCategory[] statistics;

        async void DateTimeDateChanged(DateTime? value, string name)
        {
            await InitializeCategoryResume();
            await InitializeEndPeriods();
            StateHasChanged();
        }

        async Task FilterGroupByCategoryPeriodChanged(ChangeEventArgs e)
        {
            var selectedString = e.Value.ToString();
            FilterCategoryGroupByPeriod = string.IsNullOrWhiteSpace(selectedString) ? null : selectedString;

            await InitializeCategoryResume();
            StateHasChanged();
        }

        protected override async Task OnInitializedAsync()
        {
            var today = DateTime.Now;
            FilterDateTo = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));
            FilterDateFrom = FilterDateTo.AddYears(-1);

            await InitializeCategoryResume();
            await InitializeInstallmentResume();
            await InitializeEndPeriods();
        }

        async Task InitializeInstallmentResume()
        {
            var recurrentItems = await savingsAPI.GetRecurrentMoneyItems(null, true, null, null);
            RecurrentItems = recurrentItems.Where(x => x.EndDate.HasValue && x.EndDate.Value >= DateTime.Now && x.Type == MoneyType.InstallmentPayment).OrderBy(x => x.Note).ToArray();
            DateTime endDate = DateTime.Now.AddMonths(1);
            if (RecurrentItems.Any())
            {
                endDate = RecurrentItems.Max(x => x.EndDate.Value);
            }
            var projections = await savingsAPI.GetSavings(null, endDate, true);
            Installments = projections.Where(x => x.RecurrentMoneyItemID.HasValue && x.Amount != 0 && x.Date >= DateTime.Now).ToArray();
        }

        async Task InitializeCategoryResume()
        {
            statistics = await savingsAPI.GetCategoryResume(FilterCategoryGroupByPeriod, FilterDateFrom, FilterDateTo);
        }

        async Task InitializeEndPeriods()
        {
            var past = await savingsAPI.GetMaterializedMoneyItems(FilterDateFrom, FilterDateTo, false);
            past = past.Where(x => x.EndPeriod).ToArray();

            var projection = await savingsAPI.GetSavings(FilterDateFrom, FilterDateTo);
            projection = projection.Where(x => x.EndPeriod).ToArray();

            EndPeriods = past.Union(projection).ToArray();
        }

        async Task OpenDetails(long? category, string period)
        {
            var res = await dialogService.OpenAsync<ReportsDetail>($"Report details",
                           new Dictionary<string, object>() {
                               { "FilterDateFrom", FilterDateFrom },
                               { "FilterDateTo", FilterDateTo },
                               { "category", (long?)category },
                               { "periodPattern", FilterCategoryGroupByPeriod },
                               { "period", period }
                           },
                            new DialogOptions() { Width = "800px", Height = "600px" });
        }


        ReportCategory[] FilterStatisticsResume()
        {
            var outgoing = statistics
                .SelectMany(x => x.Data)
                .Where(x => x.Amount < 0)
                .GroupBy(x => x.Period)
                .Select(x => new ReportPeriodAmount { Period = x.Key, Amount = Math.Abs(x.Sum(y => y.Amount)) });

            var incoming = statistics
               .SelectMany(x => x.Data)
               .Where(x => x.Amount > 0)
               .GroupBy(x => x.Period)
               .Select(x => new ReportPeriodAmount { Period = x.Key, Amount = x.Sum(y => y.Amount) });

            var gain = incoming
                      .Join(outgoing, inc => inc.Period, outg => outg.Period, (incItem, outgItem) => new { incItem, outgItem })
                      .Select(x => new ReportPeriodAmount { Period = x.incItem.Period, Amount = x.incItem.Amount - x.outgItem.Amount });


            var savings = EndPeriods
                .Where(x => x.Date >= FilterDateFrom && x.Date <= FilterDateTo)
                .GroupBy(x => x.Date.ToString(FilterCategoryGroupByPeriod))
                .Select(x => new ReportPeriodAmount { Period = x.Key, Amount = (double)x.OrderByDescending(x => x.Date).First().Projection });


            return new ReportCategory[]
            {
                new ReportCategory { Category="Outgoing", Data=outgoing.OrderBy(x=>x.Period).ToArray() },
                new ReportCategory { Category="Incoming", Data=incoming.OrderBy(x=>x.Period).ToArray() },
                new ReportCategory { Category="Gain", Data=gain.OrderBy(x=>x.Period).ToArray() },
                new ReportCategory { Category="Savings", Data=savings.OrderBy(x=>x.Period).ToArray() }
            };
        }



        string FormatAsAmount(object value)
        {
            return ((double)value).ToString("N2");
        }

        string FormatAsMonth(object value)
        {
            return value?.ToString();
        }
    }

}
