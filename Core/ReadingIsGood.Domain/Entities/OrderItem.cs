using System.ComponentModel.DataAnnotations.Schema;
using ReadingIsGood.BuildingBlocks.ApplicationContext.Domain.Entities;

namespace ReadingIsGood.Domain.Entities;

[Table("OrderItems")]
public class OrderItem : AuditEntity<Guid>
{
    public Guid OrderId { get; set; }
    public Guid BookId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }


    public virtual Order Order { get; set; }
    public virtual Book Book { get; set; }
}