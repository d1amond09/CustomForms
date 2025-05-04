using CustomForms.Application.Common.Responses;
using MediatR;

namespace CustomForms.Application.UseCases.Tags.GetPopularTags;

public sealed record GetPopularTagsUseCase(int Count = 20) : IRequest<ApiBaseResponse>;
