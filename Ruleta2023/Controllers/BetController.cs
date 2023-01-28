using Microsoft.AspNetCore.Mvc;
using Ruleta2023.Business.Core.Business.Bets;
using Ruleta2023.Domain.Data.Bets;

namespace Ruleta2023.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BetController : ControllerBase
    {
        private readonly BetConfigurationBusiness betConfiguration;

        public BetController(BetConfigurationBusiness betConfiguration)
        {
            this.betConfiguration = betConfiguration;
        }

        [HttpPost("MakeBet")]
        public async Task<IActionResult> MakeBet([FromBody] BetClass bet, [FromHeader] string IdRoulette)
        {
            var response = await betConfiguration.MakeBet(bet, IdRoulette);
            return StatusCode(response.statusCode,response);
        }
    }
}
