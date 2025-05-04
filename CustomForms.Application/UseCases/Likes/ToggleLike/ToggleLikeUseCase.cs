using CustomForms.Application.Common.Responses;
using MediatR;

namespace CustomForms.Application.UseCases.Likes.ToggleLike;

public sealed record ToggleLikeUseCase(Guid TemplateId) : IRequest<ApiBaseResponse>;
