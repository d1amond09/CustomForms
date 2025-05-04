using AutoMapper;
using CustomForms.Application.Common.DTOs;
using CustomForms.Application.Common.Interfaces;
using CustomForms.Application.Common.Responses;
using MediatR;

namespace CustomForms.Application.UseCases.Templates.GetPopularPublicTemplates;

public class GetPopularPublicTemplatesHandler(IRepositoryManager repManager, IMapper mapper) : IRequestHandler<GetPopularPublicTemplatesUseCase, ApiBaseResponse>
{
	private readonly IRepositoryManager _repManager = repManager;
	private readonly IMapper _mapper = mapper;

	public async Task<ApiBaseResponse> Handle(GetPopularPublicTemplatesUseCase request, CancellationToken cancellationToken)
	{
		var templates = await _repManager.Templates.GetTopPopularPublicAsync(request.Count, cancellationToken);
		var dtos = _mapper.Map<List<TemplateBriefDto>>(templates);
		return new ApiOkResponse<List<TemplateBriefDto>>(dtos);
	}
}