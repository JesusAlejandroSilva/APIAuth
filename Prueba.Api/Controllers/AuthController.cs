using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prueba.Application.Services;

namespace Prueba.Api.Controllers
{
    [ApiController]
    [Route("api/pruebatecnica")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        public AuthController(AuthService authService) => _authService = authService;

        [HttpPost("users/login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req, CancellationToken ct)
        {
            var jwt = await _authService.LoginAndIssueJwtAsync(req.Username, req.Password, ct);
            if (jwt == null) return Unauthorized();
            return Ok(new { token = jwt });
        }

        [Authorize]
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers(CancellationToken ct)
        {
            var users = await _authService.GetExternalUsers(ct);
            return Ok(users);
        }
    }

    public record LoginRequest(string Username, string Password);
}
