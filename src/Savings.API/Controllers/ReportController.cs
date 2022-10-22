﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Savings.API.Infrastructure;
using Savings.Model;

namespace Savings.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly SavingsContext _context;

        public ReportController(SavingsContext context)
        {
            _context = context;
        }

        [HttpGet("GetCategoryResume")]
        public async Task<ActionResult<ReportCategoryData[]>> GetCategoryResume(string periodPattern, int lastMonths)
        {
            var startPeriod = DateTime.Now.AddMonths(-lastMonths);
            var endPeriod = DateTime.Now;

            var sourceMaterializedRecurrentItems = await _context.MaterializedMoneyItems
                .Include(x => x.Category)
                .Where(x => x.Date >= startPeriod && x.Date <= endPeriod && x.IsRecurrent && x.Type != MoneyType.PeriodicBudget)
                .OrderByDescending(x => x.ID).ToListAsync();

            var withdrawalID = _context.Configuration.FirstOrDefault()?.CashWithdrawalCategoryID;
            var sourceFixedItemsWithoutWithDrawal = await _context.FixedMoneyItems
                .Include(x => x.Category)
                .Where(x => x.Date >= startPeriod && x.Date <= endPeriod && x.CategoryID != withdrawalID)
                .OrderByDescending(x => x.Date).ToListAsync();

            var ris1 = sourceMaterializedRecurrentItems.Select(x => new { Type = "Materialized", x.ID, x.Amount, CatIncon = x.Category?.Icon, Category = x.Category?.Description, Period = x.Date.ToString(periodPattern) });
            var ris2 = sourceFixedItemsWithoutWithDrawal.Select(x => new { Type = "Fixed", x.ID, Amount = x.Amount ?? 0, CatIncon = x.Category?.Icon, Category = x.Category?.Description, Period = x.Date.ToString(periodPattern) });

            var union = ris1.Union(ris2);

            var categoryAmounts = union.GroupBy(x => x.Category).Where(x => x.Sum(y => y.Amount) != 0);

            var lstStatistics = new List<ReportCategoryData>();
            foreach (var category in categoryAmounts)
            {
                lstStatistics.Add(new ReportCategoryData
                {
                    Category = category.Key,
                    CategoryIcon = category.First().CatIncon,
                    Data = category
                            .GroupBy(x => x.Period)
                            .Select(x => new CategoryResumDataItem() { Period = x.Key, Amount = (double)x.Sum(y => y.Amount) })
                            .Where(x => x.Amount != 0)
                            .ToArray()
                }); ;;
            }
            return lstStatistics.OrderBy(x => x.Category).ToArray();
        }
    }
}
