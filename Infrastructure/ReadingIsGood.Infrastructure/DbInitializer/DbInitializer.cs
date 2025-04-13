using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ReadingIsGood.Domain.Constants;
using ReadingIsGood.Domain.Entities;
using ReadingIsGood.Infrastructure.Context;

namespace ReadingIsGood.Infrastructure.DbInitializer;

public class DbInitializer(
    UserManager<Customer> userManager,
    RoleManager<IdentityRole<Guid>> roleManager,
    ApplicationDbContext db) : IDbInitializer
{
    public async Task Initialize()
    {
        try
        {
            if ((await db.Database.GetPendingMigrationsAsync()).Any())
            {
                await db.Database.MigrateAsync();
            }
        }
        catch (Exception e)
        {
            // ignored
        }

        await SeedRoles();
        await SeedUsers();
    }

    private async Task SeedRoles()
    {
        var roles = new List<string>
        {
            Roles.Admin,
            Roles.Customer
        };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(role));
            }
        }
    }

    private async Task SeedUsers()
    {
        var adminUser = new Customer
        {
            UserName = "admin@admin.com",
            Email = "admin@admin.com",
            Firstname = "Admin",
            Lastname = "Admin",
            CountryCode = "+90",
            PhoneNumber = "5370000000",
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
            Address = "Admin Address",
            IsActive = true,
            IsDeleted = false
        };

        await userManager.CreateAsync(adminUser, "Ab123456**");

        var admin = await userManager.Users.FirstOrDefaultAsync(x => x.Email == adminUser.Email) ?? throw new Exception("Admin user not found");
        await userManager.AddToRoleAsync(admin, Roles.Admin);

        var customerUser = new Customer
        {
            UserName = "customer@customer.com",
            Email = "customer@customer.com",
            Firstname = "John",
            Lastname = "Doe",
            CountryCode = "+90",
            PhoneNumber = "5321111111",
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
            Address = "Customer Address",
            IsActive = true,
            IsDeleted = false
        };

        await userManager.CreateAsync(customerUser, "Ab123456**");

        var customer = await userManager.Users.FirstOrDefaultAsync(x => x.Email == customerUser.Email) ?? throw new Exception("Admin user not found");
        await userManager.AddToRoleAsync(customer, Roles.Customer);
    }
}