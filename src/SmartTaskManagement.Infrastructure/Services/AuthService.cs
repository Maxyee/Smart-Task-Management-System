using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SmartTaskManagement.Application.Common;
using SmartTaskManagement.Application.DTOs.Auth;
using SmartTaskManagement.Application.Interfaces.Repositories;
using SmartTaskManagement.Application.Interfaces.Services;
using SmartTaskManagement.Domain.Entities;
using SmartTaskManagement.Domain.Enums;


namespace SmartTaskManagement.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUnitOfWork unitOfWork,
            ITokenService tokenService,
            IPasswordHasher<User> passwordHasher,
            ILogger<AuthService> logger)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        public async Task<Response<AuthResponseDto>> RegisterAsync(RegisterDto registerDto)
        {
            try
            {
                // Check if user exists
                var existingUser = await _unitOfWork.Users
                    .FindAsync(u => u.Email == registerDto.Email || u.Username == registerDto.Username);

                if (existingUser.Any())
                {
                    return Response<AuthResponseDto>.FailureResponse(
                        "User with this email or username already exists",
                        409);
                }

                // Create new user
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Email = registerDto.Email,
                    Username = registerDto.Username,
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                    Role = UserRole.TeamMember, // Default role
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                // Hash password
                user.PasswordHash = _passwordHasher.HashPassword(user, registerDto.Password);

                await _unitOfWork.Users.AddAsync(user);
                await _unitOfWork.CompleteAsync();

                // Generate tokens
                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

                var (accessToken, refreshToken) = _tokenService.GenerateTokens(claims);

                // Save refresh token
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(
                    _tokenService.GetRefreshTokenExpiryDays());

                _unitOfWork.Users.Update(user);
                await _unitOfWork.CompleteAsync();

                var response = new AuthResponseDto
                {
                    Token = accessToken,
                    RefreshToken = refreshToken,
                    Email = user.Email,
                    Username = user.Username,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Role = user.Role.ToString(),
                    ExpiresAt = DateTime.UtcNow.AddMinutes(
                        _tokenService.GetRefreshTokenExpiryDays() * 24 * 60)
                };

                _logger.LogInformation("User {Username} registered successfully", user.Username);
                return Response<AuthResponseDto>.SuccessResponse(response, "Registration successful");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration");
                return Response<AuthResponseDto>.FailureResponse(
                    "An error occurred during registration",
                    500);
            }
        }

        public async Task<Response<AuthResponseDto>> LoginAsync(LoginDto loginDto)
        {
            try
            {
                var user = await _unitOfWork.Users
                    .FindAsync(u => u.Email == loginDto.Email)
                    .ContinueWith(t => t.Result.FirstOrDefault());

                if (user == null)
                {
                    return Response<AuthResponseDto>.FailureResponse("Invalid credentials", 401);
                }

                var passwordVerification = _passwordHasher.VerifyHashedPassword(
                    user, user.PasswordHash, loginDto.Password);

                if (passwordVerification == PasswordVerificationResult.Failed)
                {
                    return Response<AuthResponseDto>.FailureResponse("Invalid credentials", 401);
                }

                if (!user.IsActive)
                {
                    return Response<AuthResponseDto>.FailureResponse("Account is deactivated", 403);
                }

                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

                var (accessToken, refreshToken) = _tokenService.GenerateTokens(claims);

                // Update refresh token
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(
                    _tokenService.GetRefreshTokenExpiryDays());
                user.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.Users.Update(user);
                await _unitOfWork.CompleteAsync();

                var response = new AuthResponseDto
                {
                    Token = accessToken,
                    RefreshToken = refreshToken,
                    Email = user.Email,
                    Username = user.Username,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Role = user.Role.ToString(),
                    ExpiresAt = DateTime.UtcNow.AddMinutes(
                        _tokenService.GetRefreshTokenExpiryDays() * 24 * 60)
                };

                _logger.LogInformation("User {Username} logged in successfully", user.Username);
                return Response<AuthResponseDto>.SuccessResponse(response, "Login successful");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                return Response<AuthResponseDto>.FailureResponse(
                    "An error occurred during login",
                    500);
            }
        }

        public async Task<Response<AuthResponseDto>> RefreshTokenAsync(RefreshTokenDto refreshTokenDto)
        {
            try
            {
                var principal = _tokenService.GetPrincipalFromExpiredToken(refreshTokenDto.Token);
                var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Response<AuthResponseDto>.FailureResponse("Invalid token", 401);
                }

                var user = await _unitOfWork.Users.GetByIdAsync(Guid.Parse(userId));

                if (user == null || user.RefreshToken != refreshTokenDto.RefreshToken ||
                    user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                {
                    return Response<AuthResponseDto>.FailureResponse("Invalid refresh token", 401);
                }

                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

                var (newAccessToken, newRefreshToken) = _tokenService.GenerateTokens(claims);

                // Update refresh token
                user.RefreshToken = newRefreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(
                    _tokenService.GetRefreshTokenExpiryDays());
                user.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.Users.Update(user);
                await _unitOfWork.CompleteAsync();

                var response = new AuthResponseDto
                {
                    Token = newAccessToken,
                    RefreshToken = newRefreshToken,
                    Email = user.Email,
                    Username = user.Username,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Role = user.Role.ToString(),
                    ExpiresAt = DateTime.UtcNow.AddMinutes(
                        _tokenService.GetRefreshTokenExpiryDays() * 24 * 60)
                };

                return Response<AuthResponseDto>.SuccessResponse(response, "Token refreshed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during token refresh");
                return Response<AuthResponseDto>.FailureResponse(
                    "An error occurred during token refresh",
                    500);
            }
        }

        public async Task<Response<bool>> LogoutAsync(string refreshToken)
        {
            try
            {
                var user = await _unitOfWork.Users
                    .FindAsync(u => u.RefreshToken == refreshToken)
                    .ContinueWith(t => t.Result.FirstOrDefault());

                if (user != null)
                {
                    user.RefreshToken = null;
                    user.RefreshTokenExpiryTime = null;
                    user.UpdatedAt = DateTime.UtcNow;

                    _unitOfWork.Users.Update(user);
                    await _unitOfWork.CompleteAsync();
                }

                return Response<bool>.SuccessResponse(true, "Logged out successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                return Response<bool>.FailureResponse(
                    "An error occurred during logout",
                    500);
            }
        }

        public async Task<Response<bool>> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);

                if (user == null)
                {
                    return Response<bool>.FailureResponse("User not found", 404);
                }

                var passwordVerification = _passwordHasher.VerifyHashedPassword(
                    user, user.PasswordHash, currentPassword);

                if (passwordVerification == PasswordVerificationResult.Failed)
                {
                    return Response<bool>.FailureResponse("Current password is incorrect", 401);
                }

                user.PasswordHash = _passwordHasher.HashPassword(user, newPassword);
                user.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.Users.Update(user);
                await _unitOfWork.CompleteAsync();

                return Response<bool>.SuccessResponse(true, "Password changed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during password change");
                return Response<bool>.FailureResponse(
                    "An error occurred while changing password",
                    500);
            }
        }
    }
}