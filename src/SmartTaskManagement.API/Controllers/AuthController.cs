using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartTaskManagement.Application.Common;
using SmartTaskManagement.Application.DTOs.Auth;
using SmartTaskManagement.Application.Interfaces.Services;


namespace SmartTaskManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var result = await _authService.RegisterAsync(registerDto);

            if (result.Success)
            {
                return Ok(result);
            }

            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Login user and get tokens
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var result = await _authService.LoginAsync(loginDto);

            if (result.Success)
            {
                return Ok(result);
            }

            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Refresh access token
        /// </summary>
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto refreshTokenDto)
        {
            var result = await _authService.RefreshTokenAsync(refreshTokenDto);

            if (result.Success)
            {
                return Ok(result);
            }

            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Logout user
        /// </summary>
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] string refreshToken)
        {
            var result = await _authService.LogoutAsync(refreshToken);

            if (result.Success)
            {
                return Ok(result);
            }

            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Change user password
        /// </summary>
        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(
            [FromBody] ChangePasswordDto changePasswordDto)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var result = await _authService.ChangePasswordAsync(
                Guid.Parse(userId),
                changePasswordDto.CurrentPassword,
                changePasswordDto.NewPassword);

            if (result.Success)
            {
                return Ok(result);
            }

            return StatusCode(result.StatusCode, result);
        }
    }

    // DTO for change password
    public class ChangePasswordDto
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}