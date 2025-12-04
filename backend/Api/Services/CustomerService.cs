using ThreeRiversBank.Api.Models;
using ThreeRiversBank.Api.Services.Data;
using ThreeRiversBank.Api.Services.Interfaces;

namespace ThreeRiversBank.Api.Services;

public class CustomerService : ICustomerService
{
    private readonly IBankingDataStore _dataStore;

    public CustomerService(IBankingDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task<Customer?> GetCustomerByIdAsync(Guid customerId)
    {
        var customer = _dataStore.Customers.FirstOrDefault(c => c.Id == customerId);
        return Task.FromResult(customer);
    }

    public Task<CustomerProfile?> GetCustomerProfileAsync(Guid customerId)
    {
        var customer = _dataStore.Customers.FirstOrDefault(c => c.Id == customerId);
        if (customer == null) return Task.FromResult<CustomerProfile?>(null);

        var accounts = _dataStore.Accounts
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

        var profile = new CustomerProfile
        {
            Id = customer.Id,
            FullName = $"{customer.FirstName} {customer.LastName}",
            Email = customer.Email,
            CustomerNumber = customer.CustomerNumber,
            MemberSince = customer.CreatedDate,
            Accounts = accounts
        };

        return Task.FromResult<CustomerProfile?>(profile);
    }

    public Task<List<Customer>> GetAllCustomersAsync()
    {
        return Task.FromResult(_dataStore.Customers.ToList());
    }

    private static string MaskAccountNumber(string accountNumber)
    {
        if (accountNumber.Length <= 4) return accountNumber;
        return $"****{accountNumber[^4..]}";
    }
}
