namespace ReadingIsGood.BuildingBlocks.ApplicationContext.Domain.Entities;

public abstract class AuditEntity<TKey> : BaseEntity<TKey>, IEntity
{
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;

    public DateTime? CreatedDate { get; set; }
    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
    public Guid? UpdatedBy { get; set; }

    public DateTime? PassiveDate { get; set; }
    public Guid? PassiveBy { get; set; }

    public DateTime? DeletedDate { get; set; }
    public Guid? DeletedBy { get; set; }
}