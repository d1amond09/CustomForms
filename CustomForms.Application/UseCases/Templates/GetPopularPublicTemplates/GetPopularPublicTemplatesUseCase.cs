using CustomForms.Application.Common.Responses;
using MediatR;

namespace CustomForms.Application.UseCases.Templates.GetPopularPublicTemplates;

public sealed record GetPopularPublicTemplatesUseCase(int Count = 5) : IRequest<ApiBaseResponse>;
