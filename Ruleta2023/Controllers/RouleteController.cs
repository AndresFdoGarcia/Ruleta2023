using Microsoft.AspNetCore.Mvc;
using Ruleta2023.Business.Core.Business.Roulette;
using Ruleta2023.Domain.Data.Ruleta;
using Ruleta2023.Domain.Data.Users;

namespace Ruleta2023.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RouleteController : ControllerBase
    {
        private readonly RouletteConfigurationBusiness rouletteConfiguration;

        public RouleteController (RouletteConfigurationBusiness rouletteConfiguration)
        {
            this.rouletteConfiguration = rouletteConfiguration;
        }

        [HttpGet]
        public async Task<IActionResult> Healthcheck()
        {
            return Ok("Todo va bien:");
        }

        [HttpPost("CreateRoulette")]
        public async Task<IActionResult> CreateRoulette()
        {
            var response = rouletteConfiguration.CreateRoulette();
            return Ok(response.ToString());
        }

        [HttpPut("OpenRolette")]
        public async Task<IActionResult> OpenRoulette([FromHeader] string id)
        {
            var response = rouletteConfiguration.OpenRoulette(id);
            return StatusCode(response.statusCode,response);
        }

        [HttpGet("AllRouletts")]
        public async Task<IActionResult> GetAllRouletts()
        {
            List<RouletteClass> response = rouletteConfiguration.GetAllRouletts();
            if (response == null)
            {
                return StatusCode(400, response);
            }
            return StatusCode(200, response);
        }
    }
}
