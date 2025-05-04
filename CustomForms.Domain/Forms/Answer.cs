using CustomForms.Domain.Common;

namespace CustomForms.Domain.Forms;

public class Answer : BaseEntity
{
	public Guid FormId { get; private set; }
	public virtual Form Form { get; private set; } = null!; 
	public Guid QuestionId { get; private set; }
	public virtual Question Question { get; private set; } = null!; 

	public string Value { get; private set; }

	public Answer(Guid id, Guid formId, Guid questionId, string value) : base(id)
	{
		if (formId == Guid.Empty) throw new ArgumentException("Form ID cannot be empty.", nameof(formId));
		if (questionId == Guid.Empty) throw new ArgumentException("Question ID cannot be empty.", nameof(questionId));

		FormId = formId;
		QuestionId = questionId;
		Value = value ?? string.Empty;
	}

	protected Answer() { }

	internal void SetValue(string newValue)
	{
		Value = newValue ?? string.Empty;
	}
}
