using CustomForms.Application.Common.Responses;
using MediatR;

namespace CustomForms.Application.UseCases.Templates.RemoveQuestionFromTemplate;

public sealed record RemoveQuestionFromTemplateUseCase(Guid TemplateId, Guid QuestionId) : IRequest<ApiBaseResponse>;
