using CustomForms.Application.Common.Interfaces;
using CustomForms.Application.Common.Responses;
using CustomForms.Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CustomForms.Application.UseCases.Users.UnblockUser;

public class UnblockUserHandler(IRepositoryManager repManager, ICurrentUserService currentUserService, UserManager<User> userManager) : IRequestHandler<UnblockUserUseCase, ApiBaseResponse>
{
	private readonly IRepositoryManager _repManager = repManager;
	private readonly ICurrentUserService _currentUserService = currentUserService;
	private readonly UserManager<User> _userManager = userManager;

	public async Task<ApiBaseResponse> Handle(UnblockUserUseCase request, CancellationToken cancellationToken)
	{
		if (!await _currentUserService.IsAdminAsync())
		{
			return new ApiForbiddenResponse("Only administrators can unblock users.");
		}

		var userToUnblock = await _userManager.FindByIdAsync(request.UserId.ToString());
		if (userToUnblock == null)
		{
			return new ApiNotFoundResponse($"User with ID '{request.UserId}' not found.");
		}

		userToUnblock.Unblock(); 

		_repManager.Users.Update(userToUnblock);
		await _repManager.CommitAsync(cancellationToken);

		return new ApiOkResponse<string>("User unblocked successfully.");
	}
}