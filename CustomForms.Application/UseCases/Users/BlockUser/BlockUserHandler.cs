using CustomForms.Application.Common.Interfaces;
using CustomForms.Application.Common.Responses;
using CustomForms.Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CustomForms.Application.UseCases.Users.BlockUser;

public class BlockUserHandler : IRequestHandler<BlockUserUseCase, ApiBaseResponse>
{
	private readonly IRepositoryManager _repManager;
	private readonly ICurrentUserService _currentUserService;
	private readonly UserManager<User> _userManager; // Fetch user

	public BlockUserHandler(IRepositoryManager repManager, ICurrentUserService currentUserService, UserManager<User> userManager)
	{
		_repManager = repManager;
		_currentUserService = currentUserService;
		_userManager = userManager;
	}

	public async Task<ApiBaseResponse> Handle(BlockUserUseCase request, CancellationToken cancellationToken)
	{
		if (!await _currentUserService.IsAdminAsync())
		{
			return new ApiForbiddenResponse("Only administrators can block users.");
		}

		var userToBlock = await _userManager.FindByIdAsync(request.UserId.ToString());
		if (userToBlock == null)
		{
			return new ApiNotFoundResponse($"User with ID '{request.UserId}' not found.");
		}

		// Prevent admin from blocking themselves (optional safeguard)
		if (userToBlock.Id == _currentUserService.UserId)
		{
			return new ApiBadRequestResponse("Administrators cannot block themselves.");
		}

		userToBlock.Block(); // Use domain method

		// Persist changes - UserManager doesn't handle our custom 'IsBlocked' property directly
		// Use the UoW pattern via repository
		_repManager.Users.Update(userToBlock); // Mark as modified
		await _repManager.CommitAsync(cancellationToken);

		return new ApiOkResponse<string>("User blocked successfully."); // Return 200 OK
	}
}