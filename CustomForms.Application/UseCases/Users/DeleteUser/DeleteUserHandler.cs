using CustomForms.Application.Common.Interfaces;
using CustomForms.Application.Common.Responses;
using CustomForms.Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CustomForms.Application.UseCases.Users.DeleteUser;

public class DeleteUserHandler : IRequestHandler<DeleteUserUseCase, ApiBaseResponse>
{
	private readonly ICurrentUserService _currentUserService;
	private readonly UserManager<User> _userManager;

	public DeleteUserHandler(ICurrentUserService currentUserService, UserManager<User> userManager)
	{
		_currentUserService = currentUserService;
		_userManager = userManager;
	}

	public async Task<ApiBaseResponse> Handle(DeleteUserUseCase request, CancellationToken cancellationToken)
	{
		if (!await _currentUserService.IsAdminAsync())
		{
			return new ApiForbiddenResponse("Only administrators can delete users.");
		}

		var userToDelete = await _userManager.FindByIdAsync(request.UserId.ToString());
		if (userToDelete == null)
		{
			return new ApiNotFoundResponse($"User with ID '{request.UserId}' not found.");
		}

		// Prevent admin from deleting themselves
		if (userToDelete.Id == _currentUserService.UserId)
		{
			return new ApiBadRequestResponse("Administrators cannot delete themselves.");
		}

		var result = await _userManager.DeleteAsync(userToDelete);

		if (!result.Succeeded)
		{
			var errors = result.Errors.Select(e => e.Description).ToList();
			string messageErrors = string.Join(',', [.. errors]);
			return new ApiBadRequestResponse($"{messageErrors}. Failed to delete user.");
		}

		return new ApiOkResponse<string>("User deleted successfully.");
	}
}