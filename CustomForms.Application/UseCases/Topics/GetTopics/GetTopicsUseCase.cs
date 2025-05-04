using CustomForms.Application.Common.Responses;
using CustomForms.Domain.Common.RequestFeatures.ModelParameters;
using MediatR;

namespace CustomForms.Application.UseCases.Topics.GetTopics;

public sealed record GetTopicsUseCase(TopicParameters Parameters) : IRequest<ApiBaseResponse>;
