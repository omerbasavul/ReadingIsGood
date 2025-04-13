using ReadingIsGood.Domain.Enums;

namespace ReadingIsGood.Application.Models;

public class OrderModel
{
    public Guid CustomerId { get; set; }
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; }
    public List<OrderItemModel>? OrderItems { get; set; }
}