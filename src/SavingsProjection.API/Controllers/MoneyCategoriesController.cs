using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SavingsProjection.API.Infrastructure;
using SavingsProjection.Model;

namespace SavingsProjection.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MoneyCategoriesController : ControllerBase
    {
        private readonly SavingProjectionContext _context;

        public MoneyCategoriesController(SavingProjectionContext context)
        {
            _context = context;
        }

        // GET: api/MoneyCategories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MoneyCategory>>> GetMoneyCategories()
        {
            return await _context.MoneyCategories.OrderBy(x => x.Description).ToListAsync();
        }

        // GET: api/MoneyCategories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MoneyCategory>> GetMoneyCategory(long id)
        {
            var moneyCategory = await _context.MoneyCategories.FindAsync(id);

            if (moneyCategory == null)
            {
                return NotFound();
            }

            return moneyCategory;
        }

        // PUT: api/MoneyCategories/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMoneyCategory(long id, MoneyCategory moneyCategory)
        {
            if (id != moneyCategory.ID)
            {
                return BadRequest();
            }

            _context.Entry(moneyCategory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MoneyCategoryExists(id))
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

        // POST: api/MoneyCategories
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<MoneyCategory>> PostMoneyCategory(MoneyCategory moneyCategory)
        {
            _context.MoneyCategories.Add(moneyCategory);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMoneyCategory", new { id = moneyCategory.ID }, moneyCategory);
        }

        // DELETE: api/MoneyCategories/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<MoneyCategory>> DeleteMoneyCategory(long id)
        {
            var moneyCategory = await _context.MoneyCategories.FindAsync(id);
            if (moneyCategory == null)
            {
                return NotFound();
            }

            _context.MoneyCategories.Remove(moneyCategory);
            await _context.SaveChangesAsync();

            return moneyCategory;
        }

        private bool MoneyCategoryExists(long id)
        {
            return _context.MoneyCategories.Any(e => e.ID == id);
        }
    }
}
