using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SavingsProjection.API.Infrastructure;
using SavingsProjection.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Threading.Tasks;

namespace SavingsProjection.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaterializedMoneyItemsController : ControllerBase
    {
        private readonly SavingProjectionContext _context;

        public MaterializedMoneyItemsController(SavingProjectionContext context)
        {
            _context = context;
        }

        // GET: api/MaterializedMoneyItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaterializedMoneyItem>>> GetMaterializedMoneyItems(DateTime? from, DateTime? to)
        {
            var res = _context.MaterializedMoneyItems.AsQueryable();
            if (from.HasValue) res = res.Where(x => x.Date >= from);
            if (to.HasValue) res = res.Where(x => x.Date <= to);
            return await _context.MaterializedMoneyItems.OrderByDescending(x => x.ID).ToListAsync();
        }

        // GET: api/MaterializedMoneyItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MaterializedMoneyItem>> GetMaterializedMoneyItem(long id)
        {
            var materializedMoneyItem = await _context.MaterializedMoneyItems.FindAsync(id);

            if (materializedMoneyItem == null)
            {
                return NotFound();
            }

            return materializedMoneyItem;
        }

        // PUT: api/MaterializedMoneyItems/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMaterializedMoneyItem(long id, MaterializedMoneyItem materializedMoneyItem)
        {
            if (id != materializedMoneyItem.ID)
            {
                return BadRequest();
            }

            _context.Entry(materializedMoneyItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MaterializedMoneyItemExists(id))
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

        // POST: api/MaterializedMoneyItems
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<MaterializedMoneyItem>> PostMaterializedMoneyItem(MaterializedMoneyItem materializedMoneyItem)
        {
            _context.MaterializedMoneyItems.Add(materializedMoneyItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMaterializedMoneyItem", new { id = materializedMoneyItem.ID }, materializedMoneyItem);
        }

        // DELETE: api/MaterializedMoneyItems/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<MaterializedMoneyItem>> DeleteMaterializedMoneyItem(long id)
        {
            var materializedMoneyItem = await _context.MaterializedMoneyItems.FindAsync(id);
            if (materializedMoneyItem == null)
            {
                return NotFound();
            }

            _context.MaterializedMoneyItems.Remove(materializedMoneyItem);
            await _context.SaveChangesAsync();

            return materializedMoneyItem;
        }


        // DELETE: api/MaterializedMoneyItems/ToHistory/5
        [HttpDelete("ToHistory/{id}")]
        public async Task<ActionResult> DeleteMaterializedMoneyItemToHistory(long id)
        {
            var materializedMoneyItem = await _context.MaterializedMoneyItems.FindAsync(id);
            if (materializedMoneyItem == null)
            {
                return NotFound();
            }
            var previous = await _context.MaterializedMoneyItems.Where(x => x.EndPeriod && x.Date < materializedMoneyItem.Date).OrderByDescending(x => x.Date).FirstOrDefaultAsync();
            if (previous != null)
            {
                _context.MaterializedMoneyItems.RemoveRange(await _context.MaterializedMoneyItems.Where(x => x.Date > previous.Date).ToListAsync());
                await _context.SaveChangesAsync();
            }
            return Ok();
        }

        private bool MaterializedMoneyItemExists(long id)
        {
            return _context.MaterializedMoneyItems.Any(e => e.ID == id);
        }
    }
}
