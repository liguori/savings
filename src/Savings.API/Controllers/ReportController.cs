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
        public async Task<ActionResult<ReportCategoryDetail[]>> GetCategoryResumeDetail(string periodPattern, DateTime dateFrom, DateTime dateTo, long? category, string period)
        {
            IEnumerable<ReportCategoryFullDetail> details = await GetCategoryDetailsAsync(periodPattern, dateFrom, dateTo);

            var res = details
                .Where(x => x.CategoryID == category && x.Period == period)
                .Select(x => new ReportCategoryDetail { Amount = x.Amount, Date = x.Date, Description = x.Description });

            return res.ToArray();
        }

        [HttpGet("GetCategoryResume")]
        public async Task<ActionResult<ReportCategoryData[]>> GetCategoryResume(string periodPattern, DateTime dateFrom, DateTime dateTo)
        {
            var categories = _context.MoneyCategories.ToList();
            IEnumerable<ReportCategoryFullDetail> union = await GetCategoryDetailsAsync(periodPattern, dateFrom, dateTo);

            var categoryAmounts = union.GroupBy(x => x.CategoryID).Where(x => x.Sum(y => y.Amount) != 0);

            var lstStatistics = new List<ReportCategoryData>();
            foreach (var category in categoryAmounts)
            {
                var cat = categories.FirstOrDefault(x => x.ID == category.Key);
                lstStatistics.Add(new ReportCategoryData
                {
                    CategoryID = category.Key,
                    Category = cat?.Description,
                    CategoryIcon = cat?.Icon,
                    Data = category
                            .GroupBy(x => x.Period)
                            .Select(x => new CategoryResumDataItem() { Period = x.Key, Amount = (double)x.Sum(y => y.Amount) })
                            .Where(x => x.Amount != 0)
                            .ToArray()
                });
            }
            return lstStatistics.OrderBy(x => x.Category).ToArray();
        }

        private async Task<IEnumerable<ReportCategoryFullDetail>> GetCategoryDetailsAsync(string periodPattern, DateTime dateFrom, DateTime dateTo)
        {
            var projectionItems = await calculator.CalculateAsync(null, dateTo, null, false);
            var withdrawalID = _context.Configuration.FirstOrDefault()?.CashWithdrawalCategoryID;


            Expression<Func<MaterializedMoneyItem, bool>> firstLevelPredicate = (x) => x.Date >= dateFrom && x.Date <= dateTo && x.CategoryID!= withdrawalID && x.Type != MoneyType.PeriodicBudget && !x.EndPeriod && x.Subitems.Count() == 0;
            Expression<Func<MaterializedMoneyItem, bool>> secondLevelPredicate = (x) => x.Date >= dateFrom && x.Date <= dateTo && x.CategoryID != withdrawalID && x.Type != MoneyType.PeriodicBudget && !x.EndPeriod && x.Subitems.Count() > 0;
            Expression<Func<ReportCategoryFullDetail, bool>> subItemPredicate = (x) => x.Date >= dateFrom && x.Date <= dateTo && x.CategoryID != withdrawalID;

            var projectionItemsFirstLevel = projectionItems
                .Where(firstLevelPredicate.Compile())
                .Select(x => new ReportCategoryFullDetail { Type = "ProjL1", ID = x.ID, Date = x.Date, Period = x.Date.ToString(periodPattern), Description = x.Note, CategoryID = x.CategoryID, Amount = x.Amount })
                .ToList();

            var projectionItemsSecondLevel = projectionItems
                .Where(secondLevelPredicate.Compile())
                .SelectMany(x => x.Subitems, (x, subitem) => new ReportCategoryFullDetail { Type = "ProjL2", ID = subitem.ID, Date = subitem.Date, Period = subitem.Date.ToString(periodPattern), Description = subitem.Note, CategoryID = subitem.CategoryID, Amount = subitem.Amount })
                .Where(subItemPredicate.Compile())
                .ToList();

            var materializedItemsFirstLevel = await _context.MaterializedMoneyItems
                .Include(x => x.Category)
                .Where(firstLevelPredicate)
                .OrderByDescending(x => x.ID)
                .Select(x => new ReportCategoryFullDetail { Type = "MaterL1", ID = x.ID, Date = x.Date, Period = x.Date.ToString(periodPattern), Description = x.Note, CategoryID = x.CategoryID, Amount = x.Amount })
                .ToListAsync();

            var materializedItemsSecondLevel = await _context.MaterializedMoneyItems
                .Include(x => x.Category)
                .Where(secondLevelPredicate)
                .SelectMany(x => x.Subitems, (moneyItem, subitem) => new ReportCategoryFullDetail { Type = "MaterL2", ID = subitem.ID, Date = moneyItem.Date, Period = moneyItem.Date.ToString(periodPattern), Description = moneyItem.Note, CategoryID = subitem.CategoryID, Amount = subitem.Amount })
                .Where(subItemPredicate)
                .ToListAsync();

            var union = projectionItemsFirstLevel.Union(projectionItemsSecondLevel).Union(materializedItemsFirstLevel).Union(materializedItemsSecondLevel);
            return union;
        }
    }
}
