using Microsoft.AspNetCore.Mvc;
using TaskList.Api.Application.Interfaces.Services;
using TaskList.Api.Domain.Users.DTOs.AuthModels;

namespace TaskListWebApi.Controllers.AuthenticationControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthInitiateController : ControllerBase
    {
        private ILogger<AuthInitiateController> _logger;
        private ILoginAndRegisterUserService _authUserService;

        public AuthInitiateController(ILogger<AuthInitiateController> logger, ILoginAndRegisterUserService authUserService)
        {
            _logger = logger;
            _authUserService = authUserService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login(UserLogin userLogin)
        {
            LoginResponse response = await _authUserService.LoginUserAsync(userLogin);
            if (!response.IsAuthorised)
            {
                _logger.LogWarning(message: "Failed to log in new user for {Email}. Request {Method}", userLogin.Email, nameof(this.Login));
                return Unauthorized(response);
            }
            return Ok(response);
        }

        [HttpPost("register")]
        public async Task<ActionResult<RegisterResponse>> Register(UserRegister userRegister)
        {   
            RegisterResponse response = await _authUserService.RegisterNewUserAsync(userRegister);
            if (!response.IsSuccessful)
            {
                _logger.LogWarning(message: "Failed to register new user for {Email}. Request {Method}", userRegister.Email, nameof(this.Register));
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}
