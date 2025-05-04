using CustomForms.Application.Common.DTOs;
using CustomForms.Application.Common.Responses;
using MediatR;

namespace CustomForms.Application.UseCases.Templates.UpdateQuestionInTemplate;

public sealed record UpdateQuestionInTemplateUseCase(Guid TemplateId, Guid QuestionId, UpdateQuestionDto QuestionData) : IRequest<ApiBaseResponse>;
