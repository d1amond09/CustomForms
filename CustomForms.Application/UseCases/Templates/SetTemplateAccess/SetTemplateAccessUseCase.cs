using CustomForms.Application.Common.DTOs;
using CustomForms.Application.Common.Responses;
using MediatR;

namespace CustomForms.Application.UseCases.Templates.SetTemplateAccess;

public sealed record SetTemplateAccessUseCase(Guid TemplateId, SetTemplateAccessDto AccessData) : IRequest<ApiBaseResponse>;
