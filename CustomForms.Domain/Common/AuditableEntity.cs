namespace CustomForms.Domain.Common;

public abstract class AuditableEntity : BaseEntity
{
	public DateTime CreatedDate { get; protected set; }
	public DateTime? LastModifiedDate { get; protected set; }

	protected AuditableEntity(Guid id) : base(id)
	{
		CreatedDate = DateTime.UtcNow;
	}
	protected AuditableEntity() : base()
	{
		CreatedDate = DateTime.UtcNow;
	}

	public void SetModifiedDate(DateTime? date = null)
	{
		LastModifiedDate = date ?? DateTime.UtcNow;
	}
}
