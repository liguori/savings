using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Savings.API.Infrastructure;
using Savings.Model;

namespace Savings.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FederationEndpointsController : ControllerBase
    {
        private readonly SavingsContext _context;

        public FederationEndpointsController(SavingsContext context)
        {
            _context = context;
        }

        // GET: api/FederationEndpoints
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FederationEndpoint>>> GetFederationEndpoints()
        {
            return await _context.FederationEndpoints.ToListAsync();
        }

        // GET: api/FederationEndpoints/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FederationEndpoint>> GetFederationEndpoint(long id)
        {
            var endpoint = await _context.FederationEndpoints.FindAsync(id);

            if (endpoint == null)
            {
                return NotFound();
            }

            return endpoint;
        }

        // POST: api/FederationEndpoints
        [HttpPost]
        public async Task<ActionResult<FederationEndpoint>> PostFederationEndpoint(FederationEndpoint endpoint)
        {
            _context.FederationEndpoints.Add(endpoint);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFederationEndpoint", new { id = endpoint.ID }, endpoint);
        }

        // PUT: api/FederationEndpoints/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFederationEndpoint(long id, FederationEndpoint endpoint)
        {
            if (id != endpoint.ID)
            {
                return BadRequest();
            }

            _context.Entry(endpoint).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.FederationEndpoints.AnyAsync(e => e.ID == id))
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

        // DELETE: api/FederationEndpoints/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<FederationEndpoint>> DeleteFederationEndpoint(long id)
        {
            var endpoint = await _context.FederationEndpoints.FindAsync(id);
            if (endpoint == null)
            {
                return NotFound();
            }

            _context.FederationEndpoints.Remove(endpoint);
            await _context.SaveChangesAsync();

            return endpoint;
        }
    }
}
