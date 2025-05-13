using System;
using Microsoft.AspNetCore.Http;

namespace CustomForms.Application.Common.DTOs;

public record TemplateBriefDto
{
	public Guid Id { get; set; }
	public string Title { get; set; } = string.Empty;
	public string AuthorName { get; set; } = string.Empty;
	public Guid AuthorId { get; set; }
	public string TopicName { get; set; } = string.Empty;
	public DateTime CreatedDate { get; set; }
	public int FormCount { get; set; }
	public int LikeCount { get; set; }
	public int CommentCount { get; set; }
	public bool IsPublic { get; set; }
}

public record TemplateDetailsDto
{
	public Guid Id { get; set; }
	public string Title { get; set; } = string.Empty;
	public string AuthorName { get; set; } = string.Empty;
	public Guid AuthorId { get; set; }
	public string TopicName { get; set; } = string.Empty;
	public DateTime CreatedDate { get; set; }
	public int FormCount { get; set; }
	public int LikeCount { get; set; }
	public bool IsPublic { get; set; }
	public string Description { get; set; } = string.Empty;
	public Guid TopicId { get; set; }
	public DateTime? LastModifiedDate { get; set; }
	public string? ImageUrl { get; set; } = null;


	public bool CanCurrentUserManage { get; set; } = false;
	public bool CanCurrentUserFill { get; set; } = false;
	public bool LikedByCurrentUser { get; set; } = false;
	public ICollection<UserSummaryDto>? AllowedUsers { get; set; } = null;
	public ICollection<CommentDto> Comments { get; set; } = [];
	public ICollection<QuestionDto> Questions { get; set; } = [];
	public ICollection<TagDto> Tags { get; set; } = [];
}

public record CreateTemplateDto(
	string Title,
	string Description,
	Guid TopicId,
	bool IsPublic,
	List<string>? Tags,
	List<Guid>? AllowedUserIds,
	IFormFile? ImageFile 
);

public record UpdateTemplateDto(
	string Title,
	string Description,
	Guid TopicId,
	IFormFile? NewImageFile, 
	bool RemoveCurrentImage 
);
public record SetTemplateAccessDto(bool IsPublic, List<Guid>? AllowedUserIds);
public record ReorderQuestionsDto(List<Guid> OrderedQuestionIds);
