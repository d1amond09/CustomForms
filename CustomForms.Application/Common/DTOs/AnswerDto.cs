using CustomForms.Domain.Templates;

namespace CustomForms.Application.Common.DTOs;

public record AnswerDto(Guid Id, Guid QuestionId, string QuestionTitle, QuestionType QuestionType, string Value);
