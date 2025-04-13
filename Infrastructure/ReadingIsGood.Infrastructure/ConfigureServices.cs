using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReadingIsGood.Application;
using ReadingIsGood.BuildingBlocks.ApplicationContext.Common.ServiceRegistrations;
using ReadingIsGood.BuildingBlocks.EntityFrameworkCore.Common;
using ReadingIsGood.Domain.Entities;
using ReadingIsGood.Infrastructure.Context;
using ReadingIsGood.Infrastructure.DbInitializer;

namespace ReadingIsGood.Infrastructure;

public static class ConfigureServices
{
    public static void AddInfrastructureLayer(this IServiceCollection serviceCollection, ConfigurationManager configuration)
    {
        serviceCollection.AddEntityFrameworkBuildingBlocks();

        serviceCollection.AddApplicationLayer(configuration);
        serviceCollection.AddApplicationDbContext(configuration);

        serviceCollection.AddScoped<IDbInitializer, DbInitializer.DbInitializer>();
    }

    private static void AddApplicationDbContext(this IServiceCollection serviceCollection, ConfigurationManager configuration)
    {
        serviceCollection.AddApplicationContextBuildingBlocks<ApplicationDbContext>(configuration);

        serviceCollection
            .AddIdentity<Customer, IdentityRole<Guid>>(options => { options.User.RequireUniqueEmail = true; })
            .AddUserManager<UserManager<Customer>>()
            .AddRoleManager<RoleManager<IdentityRole<Guid>>>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
    }
}