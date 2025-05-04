using CustomForms.Domain.Common.RequestFeatures.ModelParameters;
using CustomForms.Application.Common.Responses;
using MediatR;

namespace CustomForms.Application.UseCases.Templates.GetTemplates;

public sealed record GetTemplatesUseCase(TemplateParameters Parameters) : IRequest<ApiBaseResponse>;
