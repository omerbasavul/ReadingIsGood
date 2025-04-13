using Mediator;
using ReadingIsGood.BuildingBlocks.Common.Wrapper;

namespace ReadingIsGood.Application.Features.Statistics.Query.GetMonthlyStatistics;

public sealed record GetMonthlyStatisticsQueryRequest(Guid CustomerId) : IRequest<BaseResponse<GetMonthlyStatisticsQueryResponse>>;