using Microsoft.Extensions.Hosting;
using Serilog;

namespace ReadingIsGood.BuildingBlocks.Logging;

public static class ConfigureServices
{
    public static void UseCustomizedSerilog(this IHostBuilder hostBuilder)
    {
        hostBuilder.UseSerilog((ctx, lc) =>
        {
            lc.ReadFrom.Configuration(ctx.Configuration).Enrich.FromLogContext();
        });
    }
}