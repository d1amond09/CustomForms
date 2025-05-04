using CustomForms.Application.Common.DTOs;
using CustomForms.Application.Common.Responses;
using MediatR;

namespace CustomForms.Application.UseCases.Templates.ReorderTemplateQuestions;

public sealed record ReorderTemplateQuestionsUseCase(Guid TemplateId, ReorderQuestionsDto ReorderData) : IRequest<ApiBaseResponse>;
