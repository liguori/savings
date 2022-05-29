using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Savings.API.Infrastructure;
using Savings.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Savings.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RecurrencyAdjustementsController : ControllerBase
    {
        private readonly SavingsContext _context;

        public RecurrencyAdjustementsController(SavingsContext context)
        {
            _context = context;
        }

        // GET: api/RecurrencyAdjustements
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RecurrencyAdjustement>>> GetRecurrencyAdjustements()
        {
            return await _context.RecurrencyAdjustements.ToListAsync();
        }


        // GET: api/RecurrencyAdjustements/ByIDRecurrencyAndDate
        [HttpGet("ByIDRecurrencyAndDate", Name = "ByIDRecurrencyAndDate")]
        public async Task<ActionResult<RecurrencyAdjustement>> GetByIDRecurrency(long idRecurrency, DateTime date)
        {
            return await _context.RecurrencyAdjustements.Where(x => x.RecurrentMoneyItemID == idRecurrency && (x.RecurrencyNewDate.HasValue && x.RecurrencyNewDate == date.Date || !x.RecurrencyNewDate.HasValue && x.RecurrencyDate == date.Date)).FirstOrDefaultAsync();
        }

        // GET: api/RecurrencyAdjustements/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RecurrencyAdjustement>> GetRecurrencyAdjustement(long id)
        {
            var recurrencyAdjustement = await _context.RecurrencyAdjustements.FindAsync(id);

            if (recurrencyAdjustement == null)
            {
                return NotFound();
            }

            return recurrencyAdjustement;
        }

        // PUT: api/RecurrencyAdjustements/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRecurrencyAdjustement(long id, RecurrencyAdjustement recurrencyAdjustement)
        {
            if (id != recurrencyAdjustement.ID)
            {
                return BadRequest();
            }

            _context.Entry(recurrencyAdjustement).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RecurrencyAdjustementExists(id))
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

        // POST: api/RecurrencyAdjustements
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<RecurrencyAdjustement>> PostRecurrencyAdjustement(RecurrencyAdjustement recurrencyAdjustement)
        {
            _context.RecurrencyAdjustements.Add(recurrencyAdjustement);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRecurrencyAdjustement", new { id = recurrencyAdjustement.ID }, recurrencyAdjustement);
        }

        // DELETE: api/RecurrencyAdjustements/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<RecurrencyAdjustement>> DeleteRecurrencyAdjustement(long id)
        {
            var recurrencyAdjustement = await _context.RecurrencyAdjustements.FindAsync(id);
            if (recurrencyAdjustement == null)
            {
                return NotFound();
            }

            _context.RecurrencyAdjustements.Remove(recurrencyAdjustement);
            await _context.SaveChangesAsync();

            return recurrencyAdjustement;
        }

        private bool RecurrencyAdjustementExists(long id)
        {
            return _context.RecurrencyAdjustements.Any(e => e.ID == id);
        }
    }
}
