using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Savings.API.Services.Abstract;
using Savings.Model;

namespace Savings.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SavingsController : ControllerBase
    {
        private readonly IProjectionCalculator calculator;
        public readonly IConfiguration configuration;


        public SavingsController(IProjectionCalculator calculator, IConfiguration configuration)
        {
            this.calculator = calculator;
            this.configuration = configuration;
        }

        // GET: api/Savings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaterializedMoneyItem>>> GetSavings(DateTime? from, DateTime? to, bool onlyInstallment = false)
        {
            return (List<MaterializedMoneyItem>)await calculator.CalculateAsync(from, to, null, onlyInstallment);
        }


        [HttpPost("ToHistory")]
        public async Task<ActionResult> PostSavingsToHistory(DateTime date)
        {
            await calculator.SaveProjectionToHistory(date);
            return Ok();
        }

        [HttpGet("Backup")]
        public async Task<ActionResult> GetBackup()
        {
            byte[] fileContent;
            using (var fs = new FileStream(configuration["DatabasePath"], FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (var ms = new MemoryStream())
                {
                    fs.CopyTo(ms);
                    fileContent = ms.ToArray();
                }
            }
            return File(fileContent, "application/octet-stream", "Database.db");
        }

    }
}
