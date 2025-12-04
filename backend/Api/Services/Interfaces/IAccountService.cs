using ThreeRiversBank.Api.Models;

namespace ThreeRiversBank.Api.Services.Interfaces;

public interface IAccountService
{
    Task<Account?> GetAccountByIdAsync(Guid accountId);
    Task<List<Account>> GetAccountsByCustomerIdAsync(Guid customerId);
    Task<List<AccountSummary>> GetAccountSummariesByCustomerIdAsync(Guid customerId);
}
