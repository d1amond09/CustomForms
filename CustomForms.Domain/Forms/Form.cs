using CustomForms.Domain.Common;
using CustomForms.Domain.Common.Exceptions;
using CustomForms.Domain.Users;

namespace CustomForms.Domain.Forms;

public class Form : AuditableEntity
{
	public Guid TemplateId { get; private set; }
	public virtual Template Template { get; private set; } = null!; 
	public Guid UserId { get; private set; } 
	public virtual User User { get; private set; } = null!; 
	public DateTime FilledDate { get; private set; }

	private readonly List<Answer> _answers = new();
	public IReadOnlyCollection<Answer> Answers => _answers.AsReadOnly();

	public Form(Guid id, Guid templateId, Guid userId, IEnumerable<Answer> answers) : base(id)
	{
		if (templateId == Guid.Empty) throw new ArgumentException("Template ID cannot be empty.", nameof(templateId));
		if (userId == Guid.Empty) throw new ArgumentException("User ID cannot be empty.", nameof(userId));
		if (answers == null || !answers.Any()) throw new ArgumentException("Form must contain answers.", nameof(answers));

		TemplateId = templateId;
		UserId = userId;
		FilledDate = DateTime.UtcNow; 

		foreach (var answer in answers)
		{
			if (answer.FormId != this.Id)
				throw new Exception($"Answer '{answer.Id}' does not belong to Form '{this.Id}'. Ensure Answer is created with the correct FormId.");
			_answers.Add(answer);
		}
	}

	protected Form() { }


	public void UpdateAnswer(Guid answerId, string newValue)
	{
		var answer = _answers.FirstOrDefault(a => a.Id == answerId);
		if (answer == null)
		{
			throw new EntityNotFoundException(nameof(Answer), answerId);
		}
		answer.SetValue(newValue);
		SetModifiedDate();
	}

	public bool CanUserView(User? user, Guid templateAuthorId)
	{
		if (user == null) return false;
		if (user.IsAdmin()) return true;
		if (user.Id == UserId) return true; 
		if (user.Id == templateAuthorId) return true; 
		return false;
	}

	public bool CanUserManage(User? user) 
	{
		if (user == null) return false;
		if (user.IsAdmin()) return true;
		return user.Id == UserId;
	}
}
