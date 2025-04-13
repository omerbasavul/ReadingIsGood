using System.Linq.Expressions;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ReadingIsGood.Application.Models;
using ReadingIsGood.BuildingBlocks.Common.CustomException;
using ReadingIsGood.BuildingBlocks.Redis.Repositories;
using ReadingIsGood.Domain.Constants;
using ReadingIsGood.Domain.Entities;

namespace ReadingIsGood.Application.Services.Customers;

public sealed class CustomerService(
    UserManager<Customer> userManager,
    IRedisBaseRepository<CustomerModel> customerRedisRepository,
    IMapper mapper) : ICustomerService
{
    public async ValueTask<CustomerModel> AuthenticateAsync(string email, string password)
    {
        var customer = await userManager.FindByEmailAsync(email);
        if (customer is null)
            throw new Exception("Customer not found");

        var customerRoles = await userManager.GetRolesAsync(customer);
        if (customerRoles is null || !customerRoles.Any())
            throw new CustomServiceException("Customer has no role", StatusCodes.Status401Unauthorized);

        if (!await userManager.CheckPasswordAsync(customer, password))
        {
            await userManager.AccessFailedAsync(customer);
            throw new CustomServiceException("Email Address or Password is incorrect or empty", StatusCodes.Status401Unauthorized);
        }

        if (customer.IsActive is false)
            throw new CustomServiceException("Customer is not active", StatusCodes.Status401Unauthorized);

        if (customer.IsDeleted is true)
            throw new CustomServiceException("Customer is deleted", StatusCodes.Status401Unauthorized);

        var customerModel = mapper.Map<CustomerModel>(customer);
        customerModel.Roles = customerRoles;

        return customerModel;
    }

    public async ValueTask<bool> CreateCustomerAsync(CustomerModel customer, string password)
    {
        try
        {
            var customerExists = await GetCustomerByEmailAsync(customer.Email);
            if (customerExists is not null)
                throw new Exception("Customer already exists");

            var applicationCustomer = mapper.Map<Customer>(customer);

            applicationCustomer.UserName = customer.Email;

            var registerResult = await userManager.CreateAsync(applicationCustomer, password);

            if (!registerResult.Succeeded)
                throw new CustomServiceException("Customer cannot created.");

            var addToRoleResult = await userManager.AddToRoleAsync(applicationCustomer, Roles.Customer);
            if (!addToRoleResult.Succeeded)
                throw new CustomServiceException("Customer cannot added to role.");

            return registerResult.Succeeded;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception("Customer cannot created.");
        }
    }

    public async ValueTask<CustomerModel?> GetCustomerByIdAsync(Guid customerId)
        => await FindAndCacheCustomerAsync(
            redisPredicate: x => x.Id == customerId && x.IsActive && !x.IsDeleted,
            dbPredicate: x => x.Id == customerId && x.IsActive && !x.IsDeleted
        );

    public async ValueTask<CustomerModel?> GetCustomerByEmailAsync(string email)
        => await FindAndCacheCustomerAsync(
            redisPredicate: x => x.Email == email && x.IsActive && !x.IsDeleted,
            dbPredicate: x => x.Email == email && x.IsActive && !x.IsDeleted
        );

    public async ValueTask<CustomerModel?> GetCustomerByPhoneNumberAsync(string phoneNumber)
        => await FindAndCacheCustomerAsync(
            redisPredicate: x => x.PhoneNumber == phoneNumber && x.IsActive && !x.IsDeleted,
            dbPredicate: x => x.PhoneNumber == phoneNumber && x.IsActive && !x.IsDeleted
        );

    private async ValueTask<CustomerModel?> FindAndCacheCustomerAsync(
        Expression<Func<CustomerModel, bool>> redisPredicate,
        Expression<Func<Customer, bool>> dbPredicate)
    {
        var customerCache = await customerRedisRepository.FirstOrDefaultAsync(redisPredicate);
        if (customerCache is not null)
            return customerCache;

        var dbCustomer = await userManager.Users.FirstOrDefaultAsync(dbPredicate);
        if (dbCustomer is null)
            return null;

        var customerModel = mapper.Map<CustomerModel>(dbCustomer);
        customerModel.RedisId = await customerRedisRepository.AddAsync(customerModel);
        return customerModel;
    }
}