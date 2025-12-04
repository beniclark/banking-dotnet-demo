using ThreeRiversBank.Api.Models;

namespace ThreeRiversBank.Api.Services.Interfaces;

public interface ICustomerService
{
    Task<Customer?> GetCustomerByIdAsync(Guid customerId);
    Task<CustomerProfile?> GetCustomerProfileAsync(Guid customerId);
    Task<List<Customer>> GetAllCustomersAsync();
}
