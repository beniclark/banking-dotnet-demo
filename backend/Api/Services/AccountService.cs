using Microsoft.EntityFrameworkCore;
using ThreeRiversBank.Api.Models;
using ThreeRiversBank.Api.Services.Data;
using ThreeRiversBank.Api.Services.Interfaces;

namespace ThreeRiversBank.Api.Services;

public class AccountService : IAccountService
{
    private readonly BankingDbContext _dbContext;

    public AccountService(BankingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Account?> GetAccountByIdAsync(Guid accountId)
    {
        return await _dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == accountId);
    }

    public async Task<List<Account>> GetAccountsByCustomerIdAsync(Guid customerId)
    {
        return await _dbContext.Accounts
            .Where(a => a.CustomerId == customerId)
            .ToListAsync();
    }

    public async Task<List<AccountSummary>> GetAccountSummariesByCustomerIdAsync(Guid customerId)
    {
        return await _dbContext.Accounts
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
    }

    private static string MaskAccountNumber(string accountNumber)
    {
        if (accountNumber.Length <= 4) return accountNumber;
        return $"****{accountNumber.Substring(accountNumber.Length - 4)}";
    }
}
