using CustomForms.Domain.Common;

namespace CustomForms.Domain.Forms;

public class Question : BaseEntity
{
	public Guid TemplateId { get; private set; }
	public virtual Template Template { get; private set; } = null!;
	public string Title { get; private set; }
	public string Description { get; private set; }
	public QuestionType Type { get; private set; }
	public int Order { get; private set; } 
	public bool ShowInResults { get; private set; } 

	public Question(Guid id, Guid templateId, string title, string description, QuestionType type, bool showInResults = true) : base(id)
	{
		if (templateId == Guid.Empty) throw new ArgumentException("Template ID cannot be empty.", nameof(templateId));
		if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Question title cannot be empty.", nameof(title));

		TemplateId = templateId;
		Title = title;
		Description = description;
		Type = type;
		Order = -1; 
		ShowInResults = showInResults;
	}

	protected Question() { }

	internal void SetOrder(int order) 
	{
		if (order < 0) throw new ArgumentOutOfRangeException(nameof(order), "Order cannot be negative.");
		Order = order;
	}

	internal void UpdateDetails(string title, string description, bool showInResults)
	{
		if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Question title cannot be empty.", nameof(title));
		Title = title;
		Description = description;
		ShowInResults = showInResults;
	}
}
