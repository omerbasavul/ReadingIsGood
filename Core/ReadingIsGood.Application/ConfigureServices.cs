using System.Reflection;
using System.Text;
using FastExpressionCompiler;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using ReadingIsGood.Application.Services.Books;
using ReadingIsGood.Application.Services.Customers;
using ReadingIsGood.Application.Services.Orders;
using ReadingIsGood.Application.Services.Statistics;
using ReadingIsGood.Application.Settings;
using ReadingIsGood.BuildingBlocks.Redis;

namespace ReadingIsGood.Application;

public static class ConfigureServices
{
    public static void AddApplicationLayer(this IServiceCollection serviceCollection, ConfigurationManager configuration)
    {
        #region Configuration Bindings

        AuthenticationSettings.ValidAudiences = configuration.GetSection("AuthenticationSettings:ValidAudiences").Get<List<string>>()!;
        AuthenticationSettings.ValidIssuer = configuration["AuthenticationSettings:ValidIssuer"]!;
        AuthenticationSettings.TokenExpirationHour = configuration.GetValue<int>("AuthenticationSettings:TokenExpirationHour");
        AuthenticationSettings.Secret = configuration["AuthenticationSettings:Secret"]!;

        #endregion

        serviceCollection.AddBuildingBlocksRedis(configuration);
        serviceCollection.AddExternalDepends();

        serviceCollection.AddScoped<ICustomerService, CustomerService>();
        serviceCollection.AddScoped<IBookService, BookService>();
        serviceCollection.AddScoped<IOrderService, OrderService>();
        serviceCollection.AddScoped<IStatisticsService, StatisticsService>();
    }

    private static void AddExternalDepends(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddMediator(opts => opts.ServiceLifetime = ServiceLifetime.Scoped);
        serviceCollection.AddSingleton(GetConfiguredMappingConfig([Assembly.GetExecutingAssembly()])).AddScoped<IMapper, ServiceMapper>();

        serviceCollection
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.ClaimsIssuer = AuthenticationSettings.ValidIssuer;
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidAudiences = AuthenticationSettings.ValidAudiences,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AuthenticationSettings.Secret)),
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidIssuer = options.ClaimsIssuer,
                    ValidateAudience = true,
                    ValidAudience = options.Audience,
                    ClockSkew = TimeSpan.Zero
                };
            });
    }

    private static TypeAdapterConfig GetConfiguredMappingConfig(Assembly[] assemblies)
    {
        var config = TypeAdapterConfig.GlobalSettings;
        config.Compiler = exp => exp.CompileFast();
        config.Apply(config.Scan(assemblies));

        return config;
    }
}