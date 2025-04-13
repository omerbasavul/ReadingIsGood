using System.ComponentModel.DataAnnotations.Schema;
using ReadingIsGood.BuildingBlocks.ApplicationContext.Domain.Entities;
using ReadingIsGood.Domain.Enums;

namespace ReadingIsGood.Domain.Entities;

[Table("Orders")]
public class Order : AuditEntity<Guid>
{
    public Guid CustomerId { get; set; }
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; }

    public virtual Customer Customer { get; set; }
    public virtual ICollection<OrderItem> OrderItems { get; set; }
}