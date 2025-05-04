using CustomForms.Application.Common.Responses;
using MediatR;

namespace CustomForms.Application.UseCases.Templates.DeleteTemplate;

public sealed record DeleteTemplateUseCase(Guid TemplateId) : IRequest<ApiBaseResponse>;