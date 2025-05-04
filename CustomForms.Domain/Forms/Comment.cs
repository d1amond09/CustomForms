using CustomForms.Domain.Common;
using CustomForms.Domain.Users;

namespace CustomForms.Domain.Forms;

public class Comment : AuditableEntity
{
	public Guid TemplateId { get; private set; }
	public virtual Template Template { get; private set; } = null!; 
	public Guid UserId { get; private set; }
	public virtual User User { get; private set; } = null!; 
	public string Text { get; private set; }

	public Comment(Guid id, Guid templateId, Guid userId, string text) : base(id)
	{
		if (templateId == Guid.Empty) throw new ArgumentException("Template ID cannot be empty.", nameof(templateId));
		if (userId == Guid.Empty) throw new ArgumentException("User ID cannot be empty.", nameof(userId));
		if (string.IsNullOrWhiteSpace(text)) throw new ArgumentException("Comment text cannot be empty.", nameof(text));

		TemplateId = templateId;
		UserId = userId;
		Text = text;
	}

	protected Comment() { }
}
