using ThreeRiversBank.Api.Models;

namespace ThreeRiversBank.Api.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
}
