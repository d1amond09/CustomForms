using CustomForms.Application.Common.DTOs;
using CustomForms.Application.Common.Interfaces;
using CustomForms.Application.Common.Responses;
using CustomForms.Domain.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CustomForms.Application.UseCases.Templates.SetTemplateAccess;

public class SetTemplateAccessHandler(IRepositoryManager repManager, ICurrentUserService currentUserService) : IRequestHandler<SetTemplateAccessUseCase, ApiBaseResponse>
{
	private readonly IRepositoryManager _repManager = repManager;
	private readonly ICurrentUserService _currentUserService = currentUserService;

	public async Task<ApiBaseResponse> Handle(SetTemplateAccessUseCase request, CancellationToken cancellationToken)
	{
		var template = await _repManager.Templates
		   .FindByCondition(t => t.Id == request.TemplateId, trackChanges: true)
		   .Include(t => t.AllowedUsers) 
		   .FirstOrDefaultAsync(cancellationToken);

		if (template == null)
		{
			return new ApiNotFoundResponse($"Template with ID '{request.TemplateId}' not found.");
		}

		var user = await _currentUserService.GetCurrentUserAsync(cancellationToken);

		if (!template.CanUserManage(user))
		{
			return new ApiForbiddenResponse("You do not have permission to modify this template.");
		}

		List<User>? allowedUsers = null;
		if (!request.AccessData.IsPublic)
		{
			if (request.AccessData.AllowedUserIds == null || !request.AccessData.AllowedUserIds.Any())
			{
				return new ApiBadRequestResponse("Allowed users must be specified for restricted templates.");
			}

			allowedUsers = await _repManager.Users.GetUsersAsQueryable()
								   .Where(u => request.AccessData.AllowedUserIds.Contains(u.Id))
								   .ToListAsync(cancellationToken);

			if (allowedUsers.Count != request.AccessData.AllowedUserIds.Distinct().Count())
			{
				return new ApiBadRequestResponse("One or more allowed user IDs are invalid.");
			}
		}

		template.SetAccess(request.AccessData.IsPublic, allowedUsers);

		await _repManager.CommitAsync(cancellationToken);

		return new ApiOkResponse<string>("Template access settings updated successfully.");
	}
}