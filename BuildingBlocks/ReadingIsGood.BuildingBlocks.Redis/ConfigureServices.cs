using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReadingIsGood.BuildingBlocks.Redis.Repositories;
using Redis.OM;
using Redis.OM.Contracts;
using Redis.OM.Modeling;

namespace ReadingIsGood.BuildingBlocks.Redis;

public static class ConfigureServices
{
    public static void AddBuildingBlocksRedis(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        var connectionString = configuration["Redis:ConnectionString"] ?? throw new InvalidOperationException("Redis connection string missing.");
        var provider = new RedisConnectionProvider(connectionString);

        serviceCollection.AddSingleton<IRedisConnectionProvider>(provider);

        serviceCollection.AddScoped(typeof(IRedisBaseRepository<>), typeof(RedisBaseRepository<>));

        provider.EnsureIndexes();
    }

    private static void EnsureIndexes(this IRedisConnectionProvider provider)
    {
        var types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly =>
            {
                try
                {
                    return assembly.GetTypes();
                }
                catch
                {
                    return Array.Empty<Type>();
                }
            })
            .Where(t => t.GetCustomAttribute<DocumentAttribute>() is not null)
            .ToList();

        foreach (var type in types)
        {
            var conn = provider.Connection;

            if (conn.IsIndexCurrent(type)) continue;

            conn.DropIndex(type);
            conn.CreateIndex(type);

            Console.WriteLine($"[Redis] Indexed document: {type.Name}");
        }
    }
}