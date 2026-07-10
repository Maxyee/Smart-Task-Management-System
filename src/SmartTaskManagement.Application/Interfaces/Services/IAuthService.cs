using SmartTaskManagement.Application.Common;
using SmartTaskManagement.Application.DTOs.Auth;

namespace SmartTaskManagement.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<Response<AuthResponseDto>> RegisterAsync(RegisterDto registerDto);
        Task<Response<AuthResponseDto>> LoginAsync(LoginDto loginDto);
        Task<Response<AuthResponseDto>> RefreshTokenAsync(RefreshTokenDto refreshTokenDto);
        Task<Response<bool>> LogoutAsync(string refreshToken);
        Task<Response<bool>> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword);
    }

}