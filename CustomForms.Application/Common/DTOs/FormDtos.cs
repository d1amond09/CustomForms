namespace CustomForms.Application.Common.DTOs;

public record FormBriefDto
{
	public Guid Id { get; set; }
	public Guid TemplateId { get; set; }
	public string TemplateTitle { get; set; }
	public Guid UserId { get; set; }
	public string UserName { get; set; }
	public DateTime FilledDate { get; set; }
}
public record FormDetailsDto
{
	public Guid Id { get; set; }
	public Guid TemplateId { get; set; }
	public string TemplateTitle { get; set; } = string.Empty;
	public Guid UserId { get; set; }
	public string UserName { get; set; } = string.Empty;
	public DateTime FilledDate { get; set; }
	public ICollection<AnswerDto> Answers { get; set; } = [];
	public bool CanCurrentUserManage { get; set; } = false;
}
public record SubmitFormDto(Guid TemplateId, List<SubmitAnswerDto> Answers);
public record SubmitAnswerDto(Guid QuestionId, string Value);
