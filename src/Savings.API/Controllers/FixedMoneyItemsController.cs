using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Savings.API.Infrastructure;
using Savings.Model;

namespace Savings.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FixedMoneyItemsController : ControllerBase
    {
        private readonly SavingsContext _context;

        public FixedMoneyItemsController(SavingsContext context)
        {
            _context = context;
        }

        // GET: api/FixedMoneyItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FixedMoneyItem>>> GetFixedMoneyItems(DateTime? from, DateTime? to, bool excludeWithdrawal, long? filterCategory, bool? showToVerifyOnly)
        {
            var withdrawalID = _context.Configuration.FirstOrDefault()?.CashWithdrawalCategoryID;
            var result = _context.FixedMoneyItems.Include(x => x.Category).AsQueryable();
            if (from.HasValue) result = result.Where(x => x.Date >= from);
            if (to.HasValue) result = result.Where(x => x.Date <= to);
            if (filterCategory.HasValue) result = result.Where(x => x.CategoryID == filterCategory);
            if (excludeWithdrawal) result = result.Where(x => x.CategoryID != withdrawalID);
            if (showToVerifyOnly == true) result = result.Where(x => x.ToVerify);
            return await result.OrderByDescending(x => x.Date).ToListAsync();
        }

        // GET: api/FixedMoneyItems/ToVerify
        [HttpGet("ToVerify")]
        public async Task<ActionResult<IEnumerable<FixedMoneyItem>>> GetFixedMoneyItemsToVerify()
        {
            var items = await _context.FixedMoneyItems
                .Include(x => x.Category)
                .Where(x => x.ToVerify)
                .OrderByDescending(x => x.Date)
                .ToListAsync();
            return items;
        }

        // GET: api/FixedMoneyItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FixedMoneyItem>> GetFixedMoneyItem(long id)
        {
            var fixedMoneyItem = await _context.FixedMoneyItems.FindAsync(id);

            if (fixedMoneyItem == null)
            {
                return NotFound();
            }

            return fixedMoneyItem;
        }

        // PUT: api/FixedMoneyItems/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFixedMoneyItem(long id, FixedMoneyItem fixedMoneyItem)
        {
            if (id != fixedMoneyItem.ID)
            {
                return BadRequest();
            }

            _context.Entry(fixedMoneyItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FixedMoneyItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/FixedMoneyItems
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<FixedMoneyItem>> PostFixedMoneyItem(FixedMoneyItem fixedMoneyItem)
        {
            _context.FixedMoneyItems.Add(fixedMoneyItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFixedMoneyItem", new { id = fixedMoneyItem.ID }, fixedMoneyItem);
        }

        // DELETE: api/FixedMoneyItems/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<FixedMoneyItem>> DeleteFixedMoneyItem(long id)
        {
            var fixedMoneyItem = await _context.FixedMoneyItems.FindAsync(id);
            if (fixedMoneyItem == null)
            {
                return NotFound();
            }

            _context.FixedMoneyItems.Remove(fixedMoneyItem);
            await _context.SaveChangesAsync();

            return fixedMoneyItem;
        }

        private bool FixedMoneyItemExists(long id)
        {
            return _context.FixedMoneyItems.Any(e => e.ID == id);
        }
    }
}
