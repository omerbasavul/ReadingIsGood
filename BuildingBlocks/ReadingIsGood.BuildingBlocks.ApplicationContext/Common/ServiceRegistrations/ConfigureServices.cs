using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReadingIsGood.BuildingBlocks.ApplicationContext.Common.Configuration;
using ReadingIsGood.BuildingBlocks.ApplicationContext.Infrastructure.Context;

namespace ReadingIsGood.BuildingBlocks.ApplicationContext.Common.ServiceRegistrations;

public static class ConfigureServices
{
    public static void AddApplicationContextBuildingBlocks<TContext>(this IServiceCollection serviceCollection, ConfigurationManager configuration) where TContext : DbContext
    {
        var applicationSettings = configuration.GetSection(nameof(ApplicationSettings)).Get<ApplicationSettings>() ?? throw new InvalidOperationException("ApplicationSettings configuration not found");
        serviceCollection.AddSingleton(applicationSettings);

        serviceCollection.AddDbContextPool<TContext>(options => options.UseNpgsql(applicationSettings.ConnectionString));

        serviceCollection.AddScoped<IApplicationContext, ApplicationContext<TContext>>();
    }
}