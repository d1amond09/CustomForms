using CustomForms.Domain.Common.RequestFeatures.ModelParameters;
using CustomForms.Application.Common.Responses;
using MediatR;
using AutoMapper;
using CustomForms.Application.Common.DTOs;
using CustomForms.Application.Common.Interfaces;
using CustomForms.Domain.Common.RequestFeatures;

namespace CustomForms.Application.UseCases.Templates.GetTemplates;

public class GetTemplatesHandler(IRepositoryManager repManager, IMapper mapper, ICurrentUserService currentUserService) : IRequestHandler<GetTemplatesUseCase, ApiBaseResponse>
{
	private readonly IRepositoryManager _repManager = repManager;
	private readonly IMapper _mapper = mapper;
	private readonly ICurrentUserService _currentUserService = currentUserService;

	public async Task<ApiBaseResponse> Handle(GetTemplatesUseCase request, CancellationToken cancellationToken)
	{
		var userId = _currentUserService.UserId;
		if(userId == null)
		{
			return new ApiNotFoundResponse("Not found authorize userId");
		}
		var isAdmin = await _currentUserService.IsAdminAsync();

		var pagedTemplates = await _repManager.Templates.GetTemplatesAsync((Guid) userId, isAdmin, request.Parameters, cancellationToken);

		var templateDtos = _mapper.Map<List<TemplateBriefDto>>(pagedTemplates);

		var pagedResult = new PagedList<TemplateBriefDto>(
			templateDtos,
			pagedTemplates.MetaData.TotalCount,
			pagedTemplates.MetaData.CurrentPage,
			pagedTemplates.MetaData.PageSize
		);


		return new ApiOkResponse<(List<TemplateBriefDto> Items, MetaData MetaData)>((templateDtos, pagedResult.MetaData));
	}
}