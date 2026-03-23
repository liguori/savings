using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Savings.API.Infrastructure;
using Savings.Model;

namespace Savings.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FederationApiKeysController : ControllerBase
    {
        private readonly SavingsContext _context;

        public FederationApiKeysController(SavingsContext context)
        {
            _context = context;
        }

        // GET: api/FederationApiKeys
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FederationApiKey>>> GetFederationApiKeys()
        {
            return await _context.FederationApiKeys.ToListAsync();
        }

        // POST: api/FederationApiKeys
        [HttpPost]
        public async Task<ActionResult<FederationApiKey>> PostFederationApiKey(FederationApiKey apiKey)
        {
            _context.FederationApiKeys.Add(apiKey);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetFederationApiKeys), new { id = apiKey.ID }, apiKey);
        }

        // DELETE: api/FederationApiKeys/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<FederationApiKey>> DeleteFederationApiKey(long id)
        {
            var apiKey = await _context.FederationApiKeys.FindAsync(id);
            if (apiKey == null)
            {
                return NotFound();
            }

            _context.FederationApiKeys.Remove(apiKey);
            await _context.SaveChangesAsync();

            return apiKey;
        }
    }
}
