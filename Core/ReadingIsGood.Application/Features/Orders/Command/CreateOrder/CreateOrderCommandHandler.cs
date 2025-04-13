using Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ReadingIsGood.Application.Services.Orders;
using ReadingIsGood.BuildingBlocks.Common.CustomException;
using ReadingIsGood.BuildingBlocks.Common.Wrapper;

namespace ReadingIsGood.Application.Features.Orders.Command.CreateOrder;

public sealed class CreateOrderCommandHandler(
    IOrderService orderService,
    ILogger<CreateOrderCommandHandler> logger
) : IRequestHandler<CreateOrderInnerCommandRequest, BaseResponse<CreateOrderCommandResponse>>
{
    public async ValueTask<BaseResponse<CreateOrderCommandResponse>> Handle(CreateOrderInnerCommandRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var created = await orderService.CreateOrderAsync(request.CustomerId, request.BookId, request.Quantity, cancellationToken);
            return BaseResponse<CreateOrderCommandResponse>.Success(new CreateOrderCommandResponse(created), StatusCodes.Status200OK);
        }
        catch (CustomServiceException cex)
        {
            return BaseResponse<CreateOrderCommandResponse>.Fail(cex.Message, cex.StatucCodes);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"An exception occured while processing {nameof(CreateOrderCommandHandler)}");
            return BaseResponse<CreateOrderCommandResponse>.Fail($"An exception occured while processing {nameof(CreateOrderCommandHandler)}", StatusCodes.Status500InternalServerError);
        }
    }
}