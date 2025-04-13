using ReadingIsGood.Application.Models;

namespace ReadingIsGood.Application.Services.Statistics;

public interface IStatisticsService
{
    ValueTask<List<MonthlyStatisticsModel>> GetMonthlyStatisticsAsync(Guid customerId, CancellationToken cancellationToken = default);
}