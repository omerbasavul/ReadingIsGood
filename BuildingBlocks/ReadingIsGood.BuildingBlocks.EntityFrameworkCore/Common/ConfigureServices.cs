using ReadingIsGood.BuildingBlocks.EntityFrameworkCore.Infrastructure.Repositories;

namespace ReadingIsGood.BuildingBlocks.EntityFrameworkCore.Common;

public static class ConfigureServices
{
    public static void AddEntityFrameworkBuildingBlocks(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped(typeof(IBaseRepository<,>), typeof(BaseRepository<,>));
    }
}