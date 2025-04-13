using ReadingIsGood.Application.Models;

namespace ReadingIsGood.Application.Services.Customers;

public interface ICustomerService
{
    ValueTask<CustomerModel> AuthenticateAsync(string email, string password);
    ValueTask<bool> CreateCustomerAsync(CustomerModel customer, string password);
    ValueTask<CustomerModel?> GetCustomerByIdAsync(Guid customerId);
    ValueTask<CustomerModel?> GetCustomerByEmailAsync(string email);
    ValueTask<CustomerModel?> GetCustomerByPhoneNumberAsync(string phoneNumber);
}