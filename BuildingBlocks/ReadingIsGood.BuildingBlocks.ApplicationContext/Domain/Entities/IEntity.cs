namespace ReadingIsGood.BuildingBlocks.ApplicationContext.Domain.Entities;

public interface IEntity
{
    bool IsActive { get; set; }
    bool IsDeleted { get; set; }
    DateTime? CreatedDate { get; set; }
    Guid? CreatedBy { get; set; }
    DateTime? UpdatedDate { get; set; }
    Guid? UpdatedBy { get; set; }
    DateTime? DeletedDate { get; set; }
    Guid? DeletedBy { get; set; }
    DateTime? PassiveDate { get; set; }
    Guid? PassiveBy { get; set; }
}