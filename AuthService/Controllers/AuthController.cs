using AuthService.DTOs;
using Microsoft.AspNetCore.Mvc;
using AuthService.Services;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public IActionResult Register(RegisterDto dto)
        {
            var user = _userService.Register(dto.Username, dto.Email, dto.Password);
            if (user == null) return BadRequest("Username already exists");
            return Ok(new { user.Id, user.Username, user.Email });
        }

        [HttpGet("login")]
        public IActionResult Login([FromQuery] string username, [FromQuery] string password)
        {
            var token = _userService.Authenticate(username, password);
            if (token == null) return Unauthorized();
            return Ok(new { token });
        }
    }
}