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
        // SECURITY NOTE: This simple comparison is for demo purposes only
        // In production, use proper password hashing verification with BCrypt, scrypt, or Argon2
        return HashPassword(password) == passwordHash;
    }

    public static string HashPassword(string password)
    {
        // SECURITY NOTE: SHA256 without salt is NOT secure for production use
        // It's vulnerable to rainbow table attacks and should not be used for real password storage
        // For production, use BCrypt, scrypt, or Argon2 with proper salt
        // This simple implementation is for demo/educational purposes only
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}
