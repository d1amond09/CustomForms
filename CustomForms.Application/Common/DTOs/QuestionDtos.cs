using CustomForms.Domain.Forms;

namespace CustomForms.Application.Common.DTOs;

public record QuestionDto(Guid Id, string Title, string Description, QuestionType Type, int Order, bool ShowInResults);
public record CreateQuestionDto
{
	public string Title { get; set; } = string.Empty;
	public string Description { get; set; } = string.Empty;
	public QuestionType Type { get; set; }
	public bool ShowInResults { get; set; }
}

public record UpdateQuestionDto(string Title, string Description, bool ShowInResults);
