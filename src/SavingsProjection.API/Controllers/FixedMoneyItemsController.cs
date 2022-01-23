using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SavingsProjection.API.Infrastructure;
using SavingsProjection.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SavingsProjection.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FixedMoneyItemsController : ControllerBase
    {
        private readonly SavingProjectionContext _context;

        public FixedMoneyItemsController(SavingProjectionContext context)
        {
            _context = context;
        }

        // GET: api/FixedMoneyItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FixedMoneyItem>>> GetFixedMoneyItems(DateTime? from, DateTime? to, bool excludeWithdrawal, long? filterCategory)
        {
            var withdrawalID = _context.Configuration.FirstOrDefault()?.CashWithdrawalCategoryID;
            var result = _context.FixedMoneyItems.Include(x => x.Category).AsQueryable();
            if (from.HasValue) result = result.Where(x => x.Date >= from);
            if (to.HasValue) result = result.Where(x => x.Date <= to);
            if (filterCategory.HasValue) result = result.Where(x => x.CategoryID == filterCategory);
            if (excludeWithdrawal) result = result.Where(x => x.CategoryID != withdrawalID);
            return await result.OrderByDescending(x => x.Date).ToListAsync();
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
