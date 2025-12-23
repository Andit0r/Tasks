using Microsoft.AspNetCore.Mvc;
using ShoesMarketDbLibrary.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(AuthService authService) : ControllerBase
    {
        private readonly AuthService _authService = authService;

        // POST api/<AuthController>
        [HttpPost("login")]
        public ActionResult Post(string login, string password)
        {
            var token = _authService.LoginUser(login, password);

            return token == null
                ? Unauthorized() : Ok(new { token });
        }
    }
}
