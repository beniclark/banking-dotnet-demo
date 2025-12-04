using Microsoft.EntityFrameworkCore;
using ThreeRiversBank.Api.Models;
using ThreeRiversBank.Api.Services.Data;
using ThreeRiversBank.Api.Services.Interfaces;

namespace ThreeRiversBank.Api.Services;

public class CustomerService : ICustomerService
{
    private readonly BankingDbContext _dbContext;

    public CustomerService(BankingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Customer?> GetCustomerByIdAsync(Guid customerId)
    {
        return await _dbContext.Customers.FirstOrDefaultAsync(c => c.Id == customerId);
    }

    public async Task<CustomerProfile?> GetCustomerProfileAsync(Guid customerId)
    {
        var customer = await _dbContext.Customers.FirstOrDefaultAsync(c => c.Id == customerId);
        if (customer == null) return null;

        var accounts = await _dbContext.Accounts
            .Where(a => a.CustomerId == customerId)
            .Select(a => new AccountSummary
            {
                Id = a.Id,
                AccountNumber = MaskAccountNumber(a.AccountNumber),
                AccountType = a.AccountType,
                AccountName = a.AccountName,
                Balance = a.Balance
            })
            .ToListAsync();

        return new CustomerProfile
        {
            Id = customer.Id,
            FullName = $"{customer.FirstName} {customer.LastName}",
            Email = customer.Email,
            CustomerNumber = customer.CustomerNumber,
            MemberSince = customer.CreatedDate,
            Accounts = accounts
        };
    }

    public async Task<List<Customer>> GetAllCustomersAsync()
    {
        return await _dbContext.Customers.ToListAsync();
    }

    private static string MaskAccountNumber(string accountNumber)
    {
        if (accountNumber.Length <= 4) return accountNumber;
        return $"****{accountNumber.Substring(accountNumber.Length - 4)}";
    }
}
