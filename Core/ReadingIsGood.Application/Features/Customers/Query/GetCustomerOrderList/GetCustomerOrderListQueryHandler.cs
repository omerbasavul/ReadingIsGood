using Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ReadingIsGood.Application.Services.Orders;
using ReadingIsGood.BuildingBlocks.Common.CustomException;
using ReadingIsGood.BuildingBlocks.Common.Wrapper;

namespace ReadingIsGood.Application.Features.Customers.Query.GetCustomerOrderList;

public sealed class GetCustomerOrderListQueryHandler(
    IOrderService orderService,
    ILogger<GetCustomerOrderListQueryHandler> logger
) : IRequestHandler<GetCustomerOrderListInnerQueryRequest, BaseResponse<GetCustomerOrderListQueryResponse>>
{
    public async ValueTask<BaseResponse<GetCustomerOrderListQueryResponse>> Handle(GetCustomerOrderListInnerQueryRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.PageNumber < 1) request.PageNumber = 1;
            if (request.PageSize is < 1 or > 100) request.PageSize = 10;
            
            var (orders, pagination) = await orderService.GetOrdersByCustomerIdAsync(request.CustomerId, request.PageNumber, request.PageSize, cancellationToken);

            return BaseResponse<GetCustomerOrderListQueryResponse>.Success(new GetCustomerOrderListQueryResponse(orders, pagination), StatusCodes.Status200OK);
        }
        catch (CustomServiceException cex)
        {
            return BaseResponse<GetCustomerOrderListQueryResponse>.Fail(cex.Message, cex.StatucCodes);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"An exception occured while processing {nameof(GetCustomerOrderListQueryHandler)}");
            return BaseResponse<GetCustomerOrderListQueryResponse>.Fail($"An exception occured while processing {nameof(GetCustomerOrderListQueryHandler)}", StatusCodes.Status500InternalServerError);
        }
    }
}