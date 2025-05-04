using AutoMapper;
using CustomForms.Application.Common.DTOs;
using CustomForms.Application.Common.Interfaces;
using CustomForms.Application.Common.Responses;
using MediatR;

namespace CustomForms.Application.UseCases.Forms.GetFormById;

public class GetFormByIdHandler(IRepositoryManager repManager, IMapper mapper, ICurrentUserService currentUserService) : IRequestHandler<GetFormByIdUseCase, ApiBaseResponse>
{
	private readonly IRepositoryManager _repManager = repManager;
	private readonly IMapper _mapper = mapper;
	private readonly ICurrentUserService _currentUserService = currentUserService;

	public async Task<ApiBaseResponse> Handle(GetFormByIdUseCase request, CancellationToken cancellationToken)
	{
		var form = await _repManager.Forms.GetByIdWithDetailsAsync(request.FormId, cancellationToken);
		if (form == null)
		{
			return new ApiNotFoundResponse($"Form with ID '{request.FormId}' not found.");
		}

		var userId = _currentUserService.UserId;
		var isAdmin = await _currentUserService.IsAdminAsync();

		bool canView = isAdmin
					|| form.UserId == userId
					|| form.Template.AuthorId == userId;

		if (!canView)
		{
			return new ApiNotFoundResponse($"Form with ID '{request.FormId}' not found.");
		}

		var formDto = _mapper.Map<FormDetailsDto>(form);

		formDto.CanCurrentUserManage = isAdmin || form.UserId == userId; 
		return new ApiOkResponse<FormDetailsDto>(formDto);
	}
}

