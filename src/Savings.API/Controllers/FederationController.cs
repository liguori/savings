using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Savings.API.Infrastructure;
using Savings.API.Services.Abstract;
using Savings.Model;
using System.Security.Cryptography;

namespace Savings.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FederationController : ControllerBase
    {
        private readonly IProjectionCalculator _calculator;
        private readonly SavingsContext _context;

        public FederationController(IProjectionCalculator calculator, SavingsContext context)
        {
            _calculator = calculator;
            _context = context;
        }

        // GET: api/Federation/Savings
        [AllowAnonymous]
        [HttpGet("Savings")]
        public async Task<ActionResult<IEnumerable<MaterializedMoneyItem>>> GetSavings(DateTime? from, DateTime? to)
        {
            var apiKey = Request.Headers["X-API-Key"].FirstOrDefault();
            if (string.IsNullOrEmpty(apiKey))
            {
                return Unauthorized();
            }

            var storedKeys = await _context.FederationApiKeys.Select(k => k.Key).ToListAsync();
            var isValid = storedKeys.Any(storedKey =>
                CryptographicOperations.FixedTimeEquals(
                    System.Text.Encoding.UTF8.GetBytes(apiKey),
                    System.Text.Encoding.UTF8.GetBytes(storedKey)));

            if (!isValid)
            {
                return Unauthorized();
            }

            return (List<MaterializedMoneyItem>)await _calculator.CalculateAsync(from, to, null, false);
        }
    }
}
