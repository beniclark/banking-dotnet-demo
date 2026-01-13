using Microsoft.EntityFrameworkCore;
using ThreeRiversBank.Api.Models;
using ThreeRiversBank.Api.Services.Data;
using ThreeRiversBank.Api.Services.Interfaces;

namespace ThreeRiversBank.Api.Services;

public class AuthService : IAuthService
{
    private readonly BankingDbContext _dbContext;
    private readonly ICustomerService _customerService;

    public AuthService(BankingDbContext dbContext, ICustomerService customerService)
    {
        _dbContext = dbContext;
        _customerService = customerService;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        // Validate email format
        if (string.IsNullOrWhiteSpace(request.Email) || !IsValidEmail(request.Email))
        {
            return new LoginResponse
            {
                Success = false,
                Message = "Please provide a valid email address."
            };
        }

        // Find customer by email
        var customer = await _dbContext.Customers
            .FirstOrDefaultAsync(c => c.Email.ToLower() == request.Email.ToLower() && c.IsActive);

        if (customer == null)
        {
            return new LoginResponse
            {
                Success = false,
                Message = "No account found with this email address."
            };
        }

        // Get customer profile with accounts
        var profile = await _customerService.GetCustomerProfileAsync(customer.Id);

        return new LoginResponse
        {
            Success = true,
            Message = "Login successful",
            Customer = profile
        };
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}
