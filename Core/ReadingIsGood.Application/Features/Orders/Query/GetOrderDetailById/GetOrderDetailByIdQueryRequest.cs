using Mediator;
using ReadingIsGood.BuildingBlocks.Common.Wrapper;

namespace ReadingIsGood.Application.Features.Orders.Query.GetOrderDetailById;

public sealed class GetOrderDetailByIdQueryRequest : IRequest<BaseResponse<GetOrderDetailByIdQueryResponse>>
{
    public Guid OrderId { get; set; }
}