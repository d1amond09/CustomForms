using AutoMapper;
using CustomForms.Application.Common.DTOs;
using CustomForms.Application.Common.Interfaces;
using CustomForms.Application.Common.Responses;
using CustomForms.Domain.Common.RequestFeatures;
using CustomForms.Domain.Common.RequestFeatures.ModelParameters;
using MediatR;

namespace CustomForms.Application.UseCases.Topics.GetTopics;

public class GetTopicsHandler(IRepositoryManager repManager, IMapper mapper) : IRequestHandler<GetTopicsUseCase, ApiBaseResponse>
{
	private readonly IRepositoryManager _repManager = repManager;
	private readonly IMapper _mapper = mapper;

	public async Task<ApiBaseResponse> Handle(GetTopicsUseCase request, CancellationToken cancellationToken)
	{
		var pagedTopics = await _repManager.Topics.GetTopicsAsync(request.Parameters, cancellationToken);

		var topicDtos = _mapper.Map<List<TopicDto>>(pagedTopics);

		var pagedResult = new PagedList<TopicDto>(
		   topicDtos,
		   pagedTopics.MetaData.TotalCount,
		   pagedTopics.MetaData.CurrentPage,
		   pagedTopics.MetaData.PageSize
	   );

		return new ApiOkResponse<PagedList<TopicDto>>(pagedResult);
	}
}
