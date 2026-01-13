using Microsoft.EntityFrameworkCore;
using ThreeRiversBank.Api.Models;
using ThreeRiversBank.Api.Services.Data;
using ThreeRiversBank.Api.Services.Interfaces;

namespace ThreeRiversBank.Api.Services;

public class AuthService : IAuthService
{
    private readonly BankingDbContext _dbContext;

    public AuthService(BankingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            return new LoginResponse
            {
                Success = false,
                Message = "Username and password are required"
            };
        }

        var user = await GetUserByUsernameAsync(request.Username);
        
        if (user == null || !user.IsActive)
        {
            return new LoginResponse
            {
                Success = false,
                Message = "Invalid username or password"
            };
        }

        // Verify password hash
        if (!VerifyPassword(request.Password, user.PasswordHash))
        {
            return new LoginResponse
            {
                Success = false,
                Message = "Invalid username or password"
            };
        }

        // Get customer information
        var customer = await _dbContext.Customers.FirstOrDefaultAsync(c => c.Id == user.CustomerId);

        return new LoginResponse
        {
            Success = true,
            Message = "Login successful",
            CustomerId = user.CustomerId,
            CustomerName = customer != null ? $"{customer.FirstName} {customer.LastName}" : null
        };
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        return await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Username == username || u.Email == username);
    }

    private static bool VerifyPassword(string password, string passwordHash)
    {
        // Simple comparison for demo purposes
        // In production, use proper password hashing like BCrypt or PBKDF2
        return HashPassword(password) == passwordHash;
    }

    public static string HashPassword(string password)
    {
        // Simple hash for demo purposes
        // In production, use proper password hashing like BCrypt or PBKDF2
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}
