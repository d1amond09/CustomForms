namespace CustomForms.Application.Common.DTOs;

public record CommentDto
{
	public Guid Id { get; set; }
	public Guid UserId { get; set; }
	public string UserName { get; set; } = string.Empty;
	public string Text { get; set; } = string.Empty;
	public DateTime CreatedDate { get; set; }
	public bool CanCurrentUserDelete { get; set; } = false;
}

public record AddCommentDto(Guid TemplateId, string Text);
