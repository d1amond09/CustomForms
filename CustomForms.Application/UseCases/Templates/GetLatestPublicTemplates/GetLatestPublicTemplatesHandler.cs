using AutoMapper;
using CustomForms.Application.Common.DTOs;
using CustomForms.Application.Common.Interfaces;
using CustomForms.Application.Common.Responses;
using MediatR;

namespace CustomForms.Application.UseCases.Templates.GetLatestPublicTemplates;

public class GetLatestPublicTemplatesHandler(IRepositoryManager repManager, IMapper mapper) : IRequestHandler<GetLatestPublicTemplatesUseCase, ApiBaseResponse>
{
	private readonly IRepositoryManager _repManager = repManager;
	private readonly IMapper _mapper = mapper;

	public async Task<ApiBaseResponse> Handle(GetLatestPublicTemplatesUseCase request, CancellationToken cancellationToken)
	{
		var templates = await _repManager.Templates.GetLatestPublicAsync(request.Count, cancellationToken);
		var dtos = _mapper.Map<List<TemplateBriefDto>>(templates);
		return new ApiOkResponse<List<TemplateBriefDto>>(dtos);
	}
}