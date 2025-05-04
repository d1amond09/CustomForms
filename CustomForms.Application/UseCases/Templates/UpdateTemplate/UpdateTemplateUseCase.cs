using CustomForms.Application.Common.DTOs;
using CustomForms.Application.Common.Responses;
using MediatR;

namespace CustomForms.Application.UseCases.Templates.UpdateTemplate;

public sealed record UpdateTemplateUseCase(Guid TemplateId, UpdateTemplateDto TemplateData) : IRequest<ApiBaseResponse>;