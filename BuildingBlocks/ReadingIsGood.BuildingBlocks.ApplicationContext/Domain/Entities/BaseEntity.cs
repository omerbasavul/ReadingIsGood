using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ReadingIsGood.BuildingBlocks.ApplicationContext.Infrastructure.Enum;

namespace ReadingIsGood.BuildingBlocks.ApplicationContext.Domain.Entities;

public class BaseEntity<TKey>
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public TKey Id { get; set; }

    [Timestamp] public uint RowVersion { get; set; }

    [NotMapped] public CustomEntityState CustomState { get; set; }
}