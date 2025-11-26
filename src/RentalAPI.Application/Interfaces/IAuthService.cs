using RentalAPI.Application.DTOs;

namespace RentalAPI.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);

    string GenerateJwtToken(string username, string role);
}
