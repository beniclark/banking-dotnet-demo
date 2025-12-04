using ThreeRiversBank.Api.Models;
using ThreeRiversBank.Api.Services.Data;
using ThreeRiversBank.Api.Services.Interfaces;

namespace ThreeRiversBank.Api.Services;

public class AccountService : IAccountService
{
    private readonly IBankingDataStore _dataStore;

    public AccountService(IBankingDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task<Account?> GetAccountByIdAsync(Guid accountId)
    {
        var account = _dataStore.Accounts.FirstOrDefault(a => a.Id == accountId);
        return Task.FromResult(account);
    }

    public Task<List<Account>> GetAccountsByCustomerIdAsync(Guid customerId)
    {
        var accounts = _dataStore.Accounts.Where(a => a.CustomerId == customerId).ToList();
        return Task.FromResult(accounts);
    }

    public Task<List<AccountSummary>> GetAccountSummariesByCustomerIdAsync(Guid customerId)
    {
        var summaries = _dataStore.Accounts
            .Where(a => a.CustomerId == customerId)
            .Select(a => new AccountSummary
            {
                Id = a.Id,
                AccountNumber = MaskAccountNumber(a.AccountNumber),
                AccountType = a.AccountType,
                AccountName = a.AccountName,
                Balance = a.Balance
            })
            .ToList();

        return Task.FromResult(summaries);
    }

    private static string MaskAccountNumber(string accountNumber)
    {
        if (accountNumber.Length <= 4) return accountNumber;
        return $"****{accountNumber[^4..]}";
    }
}
