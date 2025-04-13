using System.ComponentModel.DataAnnotations.Schema;
using ReadingIsGood.BuildingBlocks.ApplicationContext.Domain.Entities;

namespace ReadingIsGood.Domain.Entities;

[Table("Books")]
public class Book : AuditEntity<Guid>
{
    public string Title { get; set; }
    public string Author { get; set; }
    public string ISBN { get; set; }
    public string ImageUrl { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
}