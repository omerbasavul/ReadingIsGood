using ReadingIsGood.Application.Models;

namespace ReadingIsGood.Application.Features.Orders.Query.GetOrderDetailById;

public sealed record GetOrderDetailByIdQueryResponse(OrderModel Order);