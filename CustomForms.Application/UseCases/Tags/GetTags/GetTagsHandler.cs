using CustomForms.Application.Common.DTOs;
using CustomForms.Domain.Common.RequestFeatures.ModelParameters;
using CustomForms.Domain.Common.RequestFeatures;
using MediatR;
using CustomForms.Application.Common.Responses;
using AutoMapper;
using CustomForms.Application.Common.Interfaces;

namespace CustomForms.Application.UseCases.Tags.GetTags;

public class GetTagsHandler(IRepositoryManager repManager, IMapper mapper) : IRequestHandler<GetTagsUseCase, ApiBaseResponse>
{
	private readonly IRepositoryManager _repManager = repManager;
	private readonly IMapper _mapper = mapper;

	public async Task<ApiBaseResponse> Handle(GetTagsUseCase request, CancellationToken cancellationToken)
	{
		var pagedTags = await _repManager.Tags.GetTagsAsync(request.Parameters, cancellationToken);

		var tagDtos = _mapper.Map<List<TagDto>>(pagedTags);

		var pagedResult = new PagedList<TagDto>(
		   tagDtos,
		   pagedTags.MetaData.TotalCount,
		   pagedTags.MetaData.CurrentPage,
		   pagedTags.MetaData.PageSize
	   );

		return new ApiOkResponse<(List<TagDto> Items, MetaData MetaData)>((tagDtos, pagedResult.MetaData));
	}
}