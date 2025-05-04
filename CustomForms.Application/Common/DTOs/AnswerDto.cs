using CustomForms.Domain.Forms;

namespace CustomForms.Application.Common.DTOs;

public record AnswerDto(Guid Id, Guid QuestionId, string QuestionTitle, QuestionType QuestionType, string Value);
