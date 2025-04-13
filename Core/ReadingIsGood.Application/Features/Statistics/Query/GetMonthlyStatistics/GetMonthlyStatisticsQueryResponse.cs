using ReadingIsGood.Application.Models;

namespace ReadingIsGood.Application.Features.Statistics.Query.GetMonthlyStatistics;

public sealed record GetMonthlyStatisticsQueryResponse(List<MonthlyStatisticsModel> MonthlyStatistics);