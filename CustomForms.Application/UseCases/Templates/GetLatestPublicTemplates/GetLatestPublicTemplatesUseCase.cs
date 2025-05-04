using CustomForms.Application.Common.Responses;
using MediatR;

namespace CustomForms.Application.UseCases.Templates.GetLatestPublicTemplates;

public sealed record GetLatestPublicTemplatesUseCase(int Count = 5) : IRequest<ApiBaseResponse>;
