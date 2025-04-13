namespace ReadingIsGood.Application.Models;

public class OrderItemModel
{
    public Guid OrderId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public virtual BookModel Book { get; set; }
}