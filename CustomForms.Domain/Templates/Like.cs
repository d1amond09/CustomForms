using CustomForms.Domain.Common;
using CustomForms.Domain.Users;

namespace CustomForms.Domain.Templates;

public class Like : BaseEntity
{
	public Guid TemplateId { get; private set; }
	public virtual Template Template { get; private set; } = null!;
	public Guid UserId { get; private set; }
	public virtual User User { get; private set; } = null!; 
	public DateTime LikedDate { get; private set; }

	public Like(Guid id, Guid templateId, Guid userId) : base(id)
	{
		if (templateId == Guid.Empty) throw new ArgumentException("Template ID cannot be empty.", nameof(templateId));
		if (userId == Guid.Empty) throw new ArgumentException("User ID cannot be empty.", nameof(userId));

		TemplateId = templateId;
		UserId = userId;
		LikedDate = DateTime.UtcNow;
	}

	protected Like() { }
}
