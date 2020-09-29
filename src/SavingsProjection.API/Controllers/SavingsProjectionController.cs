using Microsoft.AspNetCore.Mvc;
using SavingsProjection.API.Services.Abstract;
using SavingsProjection.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SavingsProjection.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class SavingsProjectionController : ControllerBase
    {
        private readonly IProjectionCalculator calculator;

        public SavingsProjectionController(IProjectionCalculator calculator)
        {
            this.calculator = calculator;
        }

        // GET: api/SavingsProjection
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaterializedMoneyItem>>> GetSavingsProjection(DateTime? from, DateTime? to)
        {
            return (List<MaterializedMoneyItem>)await calculator.CalculateAsync(from, to);
        }


        [HttpPost("ToHistory")]
        public async Task<ActionResult> PostSavingsProjectionToHistory()
        {
            await calculator.SaveProjectionToHistory();
            return Ok();
        }

    }
}
