using EntpFlow.DTOs.auth;

namespace EntpFlow.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(LoginRequestDTO dto);
}