using CustomForms.Application.Common.DTOs;
using CustomForms.Application.Common.Responses;
using MediatR;

namespace CustomForms.Application.UseCases.Templates.CreateTemplate;

public sealed record CreateTemplateUseCase(CreateTemplateDto TemplateData) : IRequest<ApiBaseResponse>;
