﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Savings.API.Infrastructure;
using Savings.Model;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Savings.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecurrentMoneyItemsController : ControllerBase
    {
        private readonly SavingsContext _context;

        public RecurrentMoneyItemsController(SavingsContext context)
        {
            _context = context;
        }

        // GET: api/RecurrentMoneyItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RecurrentMoneyItem>>> GetRecurrentMoneyItems(long? parentItemID, bool onlyActive, DateTime? endDateFrom, DateTime? endDateTo)
        {
            var res = _context.RecurrentMoneyItems.Include(x => x.AssociatedItems).AsQueryable();
            if (onlyActive) res = res.Where(x => !x.EndDate.HasValue || x.EndDate.Value >= DateTime.Now.Date);
            if (endDateFrom.HasValue) res = res.Where(x => !x.EndDate.HasValue || x.EndDate.Value >= endDateFrom.Value);
            if (endDateTo.HasValue) res = res.Where(x => x.EndDate.HasValue && x.EndDate <= endDateTo.Value);
            if (parentItemID.HasValue)
            {
                res = res.Where(x => x.RecurrentMoneyItemID == parentItemID.Value);
            }
            else
            {
                res = res.Where(x => x.RecurrentMoneyItemID == null);
            }
            return await res.OrderBy(x => x.EndDate).ToListAsync();
        }

        // GET: api/RecurrentMoneyItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RecurrentMoneyItem>> GetRecurrentMoneyItem(long id)
        {
            var recurrentMoneyItem = await _context.RecurrentMoneyItems.FindAsync(id);

            if (recurrentMoneyItem == null)
            {
                return NotFound();
            }

            return recurrentMoneyItem;
        }

        // PUT: api/RecurrentMoneyItems/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRecurrentMoneyItem(long id, RecurrentMoneyItem recurrentMoneyItem)
        {
            if (id != recurrentMoneyItem.ID)
            {
                return BadRequest();
            }

            _context.Entry(recurrentMoneyItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RecurrentMoneyItemExists(id))
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

        // POST: api/RecurrentMoneyItems
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<RecurrentMoneyItem>> PostRecurrentMoneyItem(RecurrentMoneyItem recurrentMoneyItem)
        {
            _context.RecurrentMoneyItems.Add(recurrentMoneyItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRecurrentMoneyItem", new { id = recurrentMoneyItem.ID }, recurrentMoneyItem);
        }

        [HttpPost("Credit")]
        public async Task<ActionResult<RecurrentMoneyItem>> InsertCreditFixedMoneyItem(FixedMoneyItem fixedItem)
        {
            var defaultCreditMoneyItem = _context.RecurrentMoneyItems.FirstOrDefault(x => x.DefaultCredit);

            if (defaultCreditMoneyItem == null) return BadRequest("No default credit item");

            DateTime targetDate = DateTime.Now.AddMonths(1);
            targetDate = new DateTime(targetDate.Year, targetDate.Month, defaultCreditMoneyItem.StartDate.Day);

            var recurrentMoneyItem = new RecurrentMoneyItem { CategoryID = fixedItem.CategoryID, Amount = fixedItem.Amount.Value, Note = fixedItem.Note, RecurrentMoneyItemID = defaultCreditMoneyItem.ID, StartDate = targetDate, EndDate = targetDate };
            _context.RecurrentMoneyItems.Add(recurrentMoneyItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRecurrentMoneyItem", new { id = recurrentMoneyItem.ID }, recurrentMoneyItem);
        }

        // DELETE: api/RecurrentMoneyItems/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<RecurrentMoneyItem>> DeleteRecurrentMoneyItem(long id)
        {
            var recurrentMoneyItem = await _context.RecurrentMoneyItems.FindAsync(id);
            if (recurrentMoneyItem == null)
            {
                return NotFound();
            }

            _context.RecurrentMoneyItems.Remove(recurrentMoneyItem);
            await _context.SaveChangesAsync();

            return recurrentMoneyItem;
        }

        private bool RecurrentMoneyItemExists(long id)
        {
            return _context.RecurrentMoneyItems.Any(e => e.ID == id);
        }
    }
}
