using Mediator;
using ReadingIsGood.BuildingBlocks.Common.Wrapper;

namespace ReadingIsGood.Application.Features.Orders.Command.CreateOrder;

public class CreateOrderCommandRequest
{
    public Guid BookId { get; set; }
    public int Quantity { get; set; }
}

public sealed class CreateOrderInnerCommandRequest : CreateOrderCommandRequest, IRequest<BaseResponse<CreateOrderCommandResponse>>
{
    public Guid CustomerId { get; set; }
}