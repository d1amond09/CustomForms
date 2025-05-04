using CustomForms.Application.Common.DTOs;
using CustomForms.Application.Common.Responses;
using MediatR;

namespace CustomForms.Application.UseCases.Templates.GetTemplateById;

public sealed record GetTemplateByIdUseCase(Guid TemplateId) : IRequest<ApiBaseResponse>;
