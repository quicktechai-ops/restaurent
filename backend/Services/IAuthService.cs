using Restaurant.API.DTOs;

namespace Restaurant.API.Services;

public interface IAuthService
{
    Task<LoginResponse?> LoginSuperAdminAsync(LoginRequest request);
    Task<LoginResponse?> LoginCompanyAsync(LoginRequest request);
    Task<LoginResponse?> LoginUserAsync(LoginRequest request, int companyId);
    byte[] HashPassword(string password);
    bool VerifyPassword(string password, byte[] hash);
}
