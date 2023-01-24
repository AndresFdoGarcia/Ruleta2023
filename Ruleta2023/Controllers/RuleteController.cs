using Microsoft.AspNetCore.Mvc;

namespace Ruleta2023.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RuleteController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Healthcheck()
        {
            return Ok("Todo va bien:");
        }
    }
}
