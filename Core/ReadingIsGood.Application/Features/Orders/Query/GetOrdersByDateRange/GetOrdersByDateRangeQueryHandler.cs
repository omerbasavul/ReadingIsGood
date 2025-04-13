using Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ReadingIsGood.Application.Services.Orders;
using ReadingIsGood.BuildingBlocks.Common.CustomException;
using ReadingIsGood.BuildingBlocks.Common.Wrapper;

namespace ReadingIsGood.Application.Features.Orders.Query.GetOrdersByDateRange;

public sealed class GetOrdersByDateRangeQueryHandler(
    IOrderService orderService,
    ILogger<GetOrdersByDateRangeQueryHandler> logger
) : IRequestHandler<GetOrdersByDateRangeQueryRequest, BaseResponse<GetOrdersByDateRangeQueryResponse>>
{
    public async ValueTask<BaseResponse<GetOrdersByDateRangeQueryResponse>> Handle(GetOrdersByDateRangeQueryRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.PageNumber < 1) request.PageNumber = 1;
            if (request.PageSize is < 1 or > 100) request.PageSize = 10;
            
            var (orders, pagination) = await orderService.GetOrderListByDateRangeAsync(request.StartDate, request.EndDate, request.PageNumber, request.PageSize, cancellationToken);

            return BaseResponse<GetOrdersByDateRangeQueryResponse>.Success(new GetOrdersByDateRangeQueryResponse(orders, pagination), StatusCodes.Status200OK);
        }
        catch (CustomServiceException cex)
        {
            return BaseResponse<GetOrdersByDateRangeQueryResponse>.Fail(cex.Message, cex.StatucCodes);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"An exception occured while processing {nameof(GetOrdersByDateRangeQueryHandler)}");
            return BaseResponse<GetOrdersByDateRangeQueryResponse>.Fail($"An exception occured while processing {nameof(GetOrdersByDateRangeQueryHandler)}", StatusCodes.Status500InternalServerError);
        }
    }
}