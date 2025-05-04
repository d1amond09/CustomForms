
using AutoMapper;
using CustomForms.Application.Common.DTOs;
using CustomForms.Application.Common.Interfaces;
using CustomForms.Application.Common.Responses;
using MediatR;

namespace CustomForms.Application.UseCases.Tags.GetPopularTags;

public class GetPopularTagsHandler(IRepositoryManager repManager, IMapper mapper) : IRequestHandler<GetPopularTagsUseCase, ApiBaseResponse>
{
	private readonly IRepositoryManager _repManager = repManager;
	private readonly IMapper _mapper = mapper;

	public async Task<ApiBaseResponse> Handle(GetPopularTagsUseCase request, CancellationToken cancellationToken)
	{
		var tags = await _repManager.Tags.GetPopularTagsAsync(request.Count, cancellationToken);
		var dtos = _mapper.Map<List<TagDto>>(tags);
		return new ApiOkResponse<List<TagDto>>(dtos);
	}
}
