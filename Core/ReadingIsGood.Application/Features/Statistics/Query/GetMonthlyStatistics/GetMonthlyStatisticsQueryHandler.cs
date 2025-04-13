using Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ReadingIsGood.Application.Services.Statistics;
using ReadingIsGood.BuildingBlocks.Common.CustomException;
using ReadingIsGood.BuildingBlocks.Common.Wrapper;

namespace ReadingIsGood.Application.Features.Statistics.Query.GetMonthlyStatistics;

public sealed class GetMonthlyStatisticsQueryHandler(
    IStatisticsService statisticsService,
    ILogger<GetMonthlyStatisticsQueryHandler> logger
) : IRequestHandler<GetMonthlyStatisticsQueryRequest, BaseResponse<GetMonthlyStatisticsQueryResponse>>
{
    public async ValueTask<BaseResponse<GetMonthlyStatisticsQueryResponse>> Handle(GetMonthlyStatisticsQueryRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var monthlyStatisticsModel = await statisticsService.GetMonthlyStatisticsAsync(request.CustomerId, cancellationToken);

            return BaseResponse<GetMonthlyStatisticsQueryResponse>.Success(new GetMonthlyStatisticsQueryResponse(monthlyStatisticsModel), StatusCodes.Status200OK);
        }
        catch (CustomServiceException cex)
        {
            return BaseResponse<GetMonthlyStatisticsQueryResponse>.Fail(cex.Message, cex.StatucCodes);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"An exception occured while processing {nameof(GetMonthlyStatisticsQueryHandler)}");
            return BaseResponse<GetMonthlyStatisticsQueryResponse>.Fail($"An exception occured while processing {nameof(GetMonthlyStatisticsQueryHandler)}", StatusCodes.Status500InternalServerError);
        }
    }
}