
using CustomForms.Application.Common.DTOs;
using CustomForms.Domain.Common.RequestFeatures.ModelParameters;
using CustomForms.Domain.Common.RequestFeatures;
using MediatR;
using CustomForms.Application.Common.Responses;
using AutoMapper;
using CustomForms.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CustomForms.Application.UseCases.Forms.GetForms;

public class GetFormsHandler(IRepositoryManager repManager, IMapper mapper, ICurrentUserService currentUserService) : IRequestHandler<GetFormsUseCase, ApiBaseResponse>
{
	private readonly IRepositoryManager _repManager = repManager;
	private readonly IMapper _mapper = mapper;
	private readonly ICurrentUserService _currentUserService = currentUserService;

	public async Task<ApiBaseResponse> Handle(GetFormsUseCase request, CancellationToken cancellationToken)
	{
		var userId = _currentUserService.UserId;
		var isAdmin = await _currentUserService.IsAdminAsync();

		if (!isAdmin)
		{
			if (request.Parameters.TemplateId.HasValue)
			{
				var templateAuthorId = await _repManager.Templates
					.FindByCondition(t => t.Id == request.Parameters.TemplateId.Value, trackChanges: false)
					.Select(t => (Guid?)t.AuthorId)
					.FirstOrDefaultAsync(cancellationToken);

				if (templateAuthorId != userId)
				{
					return new ApiForbiddenResponse("You can only view forms for templates you created.");
				}
			}
			else if (request.Parameters.UserId.HasValue)
			{
				if (request.Parameters.UserId != userId)
				{
					return new ApiForbiddenResponse("You can only view your own submitted forms.");
				}
				
			}
			else
			{
				if (userId == null) 
					return new ApiForbiddenResponse("Authentication required.");
				request.Parameters.UserId = userId; 
			}
		}

		var pagedForms = await _repManager.Forms.GetFormsAsync(request.Parameters, cancellationToken);

		var formDtos = _mapper.Map<List<FormBriefDto>>(pagedForms);

		var pagedResult = new PagedList<FormBriefDto>(
		   formDtos,
		   pagedForms.MetaData.TotalCount,
		   pagedForms.MetaData.CurrentPage,
		   pagedForms.MetaData.PageSize
	   );

		return new ApiOkResponse<(List<FormBriefDto> Items, MetaData MetaData)>((formDtos, pagedForms.MetaData));
	}
}
