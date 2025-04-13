using System.Collections.Immutable;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using ReadingIsGood.Application.Models;
using ReadingIsGood.Application.Services.Books;
using ReadingIsGood.BuildingBlocks.Common.CustomException;
using ReadingIsGood.BuildingBlocks.Common.Wrapper;
using ReadingIsGood.BuildingBlocks.EntityFrameworkCore.Infrastructure.Repositories;
using ReadingIsGood.Domain.Entities;
using ReadingIsGood.Domain.Enums;

namespace ReadingIsGood.Application.Services.Orders;

public sealed class OrderService(
    IBaseRepository<Order, Guid> orderRepository,
    IBaseRepository<OrderItem, Guid> orderItemRepository,
    IBookService bookService,
    IMapper mapper
) : IOrderService
{
    public async ValueTask<Guid> CreateOrderAsync(Guid customerId, Guid bookId, int quantity, CancellationToken cancellationToken = default)
    {
        if (quantity <= 0)
            throw new CustomServiceException("Quantity must be greater than zero", StatusCodes.Status400BadRequest);

        var bookModel = await bookService.GetBookByIdAsync(bookId);

        await bookService.DecreaseStockAsync(bookId, quantity, bookModel.RowVersion);

        var order = new Order
        {
            CustomerId = customerId,
            Status = OrderStatus.Pending,
            TotalAmount = bookModel.Price * quantity
        };

        await orderRepository.AddAsync(order, cancellationToken);

        var orderItem = new OrderItem
        {
            OrderId = order.Id,
            BookId = bookId,
            Quantity = quantity,
            Price = bookModel.Price
        };

        await orderItemRepository.AddAsync(orderItem, cancellationToken);

        return order.Id;
    }

    public async ValueTask<OrderModel> GetOrderByIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var order = await orderRepository.GetAsync(x => x.Id == orderId, tracking: false, cancellationToken);
        if (order == null)
            throw new CustomServiceException($"Order not found", StatusCodes.Status404NotFound);

        var (orderItems, _) = await orderItemRepository.GetListAsync(x => x.OrderId == order.Id, pageNumber: 1, pageSize: 9999, tracking: false, cancellationToken: cancellationToken);

        var orderItemModels = new List<OrderItemModel>();
        foreach (var item in orderItems)
        {
            var bookModel = await bookService.GetBookByIdAsync(item.BookId);
            orderItemModels.Add(new OrderItemModel
            {
                OrderId = item.OrderId,
                Quantity = item.Quantity,
                Price = item.Price,
                Book = bookModel
            });
        }

        return new OrderModel
        {
            CustomerId = order.CustomerId,
            TotalAmount = order.TotalAmount,
            Status = order.Status,
            OrderItems = orderItemModels
        };
    }

    public async ValueTask<(ImmutableList<OrderModel>, PaginationResult)> GetOrderListByDateRangeAsync(DateTime startDate, DateTime endDate, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var (orders, pagination) = await orderRepository.GetListAsync(
            x => x.CreatedDate >= startDate && x.CreatedDate <= endDate,
            pageNumber: pageNumber,
            pageSize: pageSize,
            tracking: false,
            cancellationToken: cancellationToken);

        var orderModels = mapper.Map<List<OrderModel>>(orders);
        return (orderModels.ToImmutableList(), pagination);
    }

    public async ValueTask<(ImmutableList<OrderModel>, PaginationResult)> GetOrdersByCustomerIdAsync(Guid customerId, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var (orders, pagination) = await orderRepository.GetListAsync(
            x => x.CustomerId == customerId,
            pageNumber: pageNumber,
            pageSize: pageSize,
            tracking: false,
            cancellationToken: cancellationToken
        );

        var orderModels = new List<OrderModel>();

        foreach (var order in orders)
        {
            var (items, _) = await orderItemRepository.GetListAsync(
                x => x.OrderId == order.Id,
                pageNumber: 1,
                pageSize: 9999,
                tracking: false,
                cancellationToken: cancellationToken
            );

            var itemModels = new List<OrderItemModel>();
            foreach (var item in items)
            {
                var bookModel = await bookService.GetBookByIdAsync(item.BookId);
                itemModels.Add(new OrderItemModel
                {
                    OrderId = item.OrderId,
                    Quantity = item.Quantity,
                    Price = item.Price,
                    Book = bookModel
                });
            }

            orderModels.Add(new OrderModel
            {
                CustomerId = order.CustomerId,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                OrderItems = itemModels
            });
        }

        return (orderModels.ToImmutableList(), pagination);
    }
}