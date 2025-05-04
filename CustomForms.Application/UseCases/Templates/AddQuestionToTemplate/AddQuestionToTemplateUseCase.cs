using CustomForms.Application.Common.DTOs;
using CustomForms.Application.Common.Responses;
using MediatR;

namespace CustomForms.Application.UseCases.Templates.AddQuestionToTemplate;

public sealed record AddQuestionToTemplateUseCase(Guid TemplateId, CreateQuestionDto QuestionData) : IRequest<ApiBaseResponse>;
