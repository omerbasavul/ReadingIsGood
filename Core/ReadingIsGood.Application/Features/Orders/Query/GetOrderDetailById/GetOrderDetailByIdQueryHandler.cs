using Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ReadingIsGood.Application.Services.Orders;
using ReadingIsGood.BuildingBlocks.Common.CustomException;
using ReadingIsGood.BuildingBlocks.Common.Wrapper;

namespace ReadingIsGood.Application.Features.Orders.Query.GetOrderDetailById;

public sealed class GetOrderDetailByIdQueryHandler(
    IOrderService orderService,
    ILogger<GetOrderDetailByIdQueryHandler> logger
) : IRequestHandler<GetOrderDetailByIdQueryRequest, BaseResponse<GetOrderDetailByIdQueryResponse>>
{
    public async ValueTask<BaseResponse<GetOrderDetailByIdQueryResponse>> Handle(GetOrderDetailByIdQueryRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var order = await orderService.GetOrderByIdAsync(request.OrderId, cancellationToken);

            return BaseResponse<GetOrderDetailByIdQueryResponse>.Success(new GetOrderDetailByIdQueryResponse(order), StatusCodes.Status200OK);
        }
        catch (CustomServiceException cex)
        {
            return BaseResponse<GetOrderDetailByIdQueryResponse>.Fail(cex.Message, cex.StatucCodes);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"An exception occured while processing {nameof(GetOrderDetailByIdQueryHandler)}");
            return BaseResponse<GetOrderDetailByIdQueryResponse>.Fail($"An exception occured while processing {nameof(GetOrderDetailByIdQueryHandler)}", StatusCodes.Status500InternalServerError);
        }
    }
}