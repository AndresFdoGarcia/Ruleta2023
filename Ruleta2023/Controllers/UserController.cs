using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ruleta2023.Business.Core.Business.User;
using Ruleta2023.Domain.Data.Users;

namespace Ruleta2023.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserConfigurationBusiness userConfiguration;
        public UserController(UserConfigurationBusiness userConfiguration)
        {
            this.userConfiguration = userConfiguration;
        }


        [HttpPost("CreateUser")]      
        public async Task<IActionResult> Createuser([FromBody] UserClass clientClass)
        {
            var response = userConfiguration.CreateUser(clientClass);
            return StatusCode(response.statusCode, response);
        }
    }
}
