using EventManagementSystemApi.Models.DTOs;
using EventManagementSystemApi.Models;

namespace EventManagementSystemApi.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
        Task<AuthResponseDto> GenerateTokenAsync(User user);
    }
}
