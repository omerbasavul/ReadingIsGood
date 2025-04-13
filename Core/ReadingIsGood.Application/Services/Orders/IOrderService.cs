using System.Collections.Immutable;
using ReadingIsGood.Application.Models;
using ReadingIsGood.BuildingBlocks.Common.Wrapper;

namespace ReadingIsGood.Application.Services.Orders;

public interface IOrderService
{
    ValueTask<Guid> CreateOrderAsync(Guid customerId, Guid bookId, int quantity, CancellationToken cancellationToken = default);
    ValueTask<OrderModel> GetOrderByIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    ValueTask<(ImmutableList<OrderModel>, PaginationResult)> GetOrderListByDateRangeAsync(DateTime startDate, DateTime endDate, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default);
    ValueTask<(ImmutableList<OrderModel>, PaginationResult)> GetOrdersByCustomerIdAsync(Guid customerId, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default);
}