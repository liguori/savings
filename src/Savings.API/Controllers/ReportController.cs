using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Savings.API.Infrastructure;
using Savings.API.Services.Abstract;
using Savings.Model;
using System.Linq.Expressions;

namespace Savings.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly SavingsContext _context;
        private readonly IProjectionCalculator calculator;

        public ReportController(IProjectionCalculator calculator, SavingsContext context)
        {
            _context = context;
            this.calculator = calculator;
        }

        [HttpGet("GetCategoryResumeDetail")]
        public async Task<ActionResult<ReportDetail[]>> GetCategoryResumeDetail(string periodPattern, DateTime dateFrom, DateTime dateTo, long? category, string period, bool? work)
        {
            IEnumerable<ReportFullDetail> details = await GetCategoryDetailsAsync(periodPattern, dateFrom, dateTo, work);

            var res = details
                .Where(x => x.CategoryID == category && x.Period == period)
                .Select(x => new ReportDetail { Amount = x.Amount, Date = x.Date, Description = x.Description });

            return res.ToArray();
        }

        [HttpGet("GetCategoryResume")]
        public async Task<ActionResult<ReportCategory[]>> GetCategoryResume(string periodPattern, DateTime dateFrom, DateTime dateTo, bool? work)
        {
            var categories = await _context.MoneyCategories.AsNoTracking().ToDictionaryAsync(c => c.ID);
            IEnumerable<ReportFullDetail> union = await GetCategoryDetailsAsync(periodPattern, dateFrom, dateTo, work);

            var categoryAmounts = union.GroupBy(x => x.CategoryID).Where(x => x.Sum(y => y.Amount) != 0);

            var lstStatistics = new List<ReportCategory>();
            foreach (var category in categoryAmounts)
            {
                categories.TryGetValue(category.Key ?? 0, out var cat);
                lstStatistics.Add(new ReportCategory
                {
                    CategoryID = category.Key,
                    Category = cat?.Description,
                    CategoryIcon = cat?.Icon,
                    Data = category
                            .GroupBy(x => x.Period)
                            .Select(x => new ReportPeriodAmount() { Period = x.Key, Amount = (double)x.Sum(y => y.Amount) })
                            .Where(x => x.Amount != 0)
                            .ToArray()
                });
            }
            return lstStatistics.OrderBy(x => x.Category).ToArray();
        }

        private async Task<IEnumerable<ReportFullDetail>> GetCategoryDetailsAsync(string periodPattern, DateTime dateFrom, DateTime dateTo, bool? work)
        {
            var projectionItems = await calculator.CalculateAsync(null, dateTo, null, false);
            var withdrawalID = (await _context.Configuration.FirstOrDefaultAsync())?.CashWithdrawalCategoryID;


            Expression<Func<MaterializedMoneyItem, bool>> firstLevelPredicate = (x) => x.Date >= dateFrom && x.Date <= dateTo && x.CategoryID!= withdrawalID && x.Type != MoneyType.PeriodicBudget && !x.EndPeriod && x.Subitems.Count() == 0 && (!work.HasValue || x.Work == work.Value);
            Expression<Func<MaterializedMoneyItem, bool>> secondLevelPredicate = (x) => x.Date >= dateFrom && x.Date <= dateTo && x.CategoryID != withdrawalID && x.Type != MoneyType.PeriodicBudget && !x.EndPeriod && x.Subitems.Count() > 0 && (!work.HasValue || x.Work == work.Value);
            Expression<Func<ReportFullDetail, bool>> subItemPredicate = (x) => x.Date >= dateFrom && x.Date <= dateTo && x.CategoryID != withdrawalID && (!work.HasValue || x.Work == work.Value);

            // Compile once for in-memory filtering of projection items
            var firstLevelFunc = firstLevelPredicate.Compile();
            var secondLevelFunc = secondLevelPredicate.Compile();
            var subItemFunc = subItemPredicate.Compile();

            var projectionItemsFirstLevel = projectionItems
                .Where(firstLevelFunc)
                .Select(x => new ReportFullDetail { Type = "ProjL1", ID = x.ID, Date = x.Date, Period = x.Date.ToString(periodPattern), Description = x.Note, CategoryID = x.CategoryID, Amount = x.Amount, Work = x.Work })
                .ToList();

            var projectionItemsSecondLevel = projectionItems
                .Where(secondLevelFunc)
                .SelectMany(x => x.Subitems, (x, subitem) => new ReportFullDetail { Type = "ProjL2", ID = subitem.ID, Date = subitem.Date, Period = subitem.Date.ToString(periodPattern), Description = subitem.Note, CategoryID = subitem.CategoryID, Amount = subitem.Amount, Work = x.Work || subitem.Work })
                .Where(subItemFunc)
                .ToList();

            var materializedItemsFirstLevel = await _context.MaterializedMoneyItems
                .Include(x => x.Category)
                .AsNoTracking()
                .Where(firstLevelPredicate)
                .OrderByDescending(x => x.ID)
                .Select(x => new ReportFullDetail { Type = "MaterL1", ID = x.ID, Date = x.Date, Period = x.Date.ToString(periodPattern), Description = x.Note, CategoryID = x.CategoryID, Amount = x.Amount, Work = x.Work })
                .ToListAsync();

            var materializedItemsSecondLevel = await _context.MaterializedMoneyItems
                .Include(x => x.Category)
                .AsNoTracking()
                .Where(secondLevelPredicate)
                .SelectMany(x => x.Subitems, (moneyItem, subitem) => new ReportFullDetail { Type = "MaterL2", ID = subitem.ID, Date = subitem.Date, Period = subitem.Date.ToString(periodPattern), Description = subitem.Note, CategoryID = subitem.CategoryID, Amount = subitem.Amount, Work = moneyItem.Work || subitem.Work })
                .Where(subItemPredicate)
                .ToListAsync();

            var union = projectionItemsFirstLevel.Union(projectionItemsSecondLevel).Union(materializedItemsFirstLevel).Union(materializedItemsSecondLevel);
            return union;
        }
    }
}
