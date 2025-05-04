using System.Linq;
using CustomForms.Application.Common.Interfaces;
using CustomForms.Application.Common.Responses;
using CustomForms.Domain.Common.Constants;
using CustomForms.Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CustomForms.Application.UseCases.Users.SetUserRole;

public class SetUserRoleHandler(ICurrentUserService currentUserService, UserManager<User> userManager, RoleManager<Role> roleManager) : IRequestHandler<SetUserRoleUseCase, ApiBaseResponse>
{
	private readonly ICurrentUserService _currentUserService = currentUserService;
	private readonly UserManager<User> _userManager = userManager;
	private readonly RoleManager<Role> _roleManager = roleManager; // To check if role exists

	public async Task<ApiBaseResponse> Handle(SetUserRoleUseCase request, CancellationToken cancellationToken)
	{
		if (!await _currentUserService.IsAdminAsync())
		{
			return new ApiForbiddenResponse("Only administrators can change user roles.");
		}

		var user = await _userManager.FindByIdAsync(request.UserId.ToString());
		if (user == null)
		{
			return new ApiNotFoundResponse($"User with ID '{request.UserId}' not found.");
		}

		// Prevent changing own role if it involves removing admin status
		if (user.Id == _currentUserService.UserId && request.RoleName == Roles.Admin && !request.AddRole)
		{
			return new ApiBadRequestResponse("Administrators cannot remove their own admin role.");
		}

		// Check if role exists
		if (!await _roleManager.RoleExistsAsync(request.RoleName))
		{
			return new ApiBadRequestResponse($"Role '{request.RoleName}' does not exist.");
		}

		IdentityResult result;
		string actionVerb = request.AddRole ? "add" : "remove";
		string preposition = request.AddRole ? "to" : "from";

		if (request.AddRole)
		{
			// Avoid adding if already in role
			if (await _userManager.IsInRoleAsync(user, request.RoleName))
			{
				return new ApiOkResponse<string>($"User is already in role '{request.RoleName}'.");
			}
			result = await _userManager.AddToRoleAsync(user, request.RoleName);
		}
		else
		{
			// Avoid removing if not in role
			if (!await _userManager.IsInRoleAsync(user, request.RoleName))
			{
				return new ApiOkResponse<string>($"User is not in role '{request.RoleName}'.");
			}
			result = await _userManager.RemoveFromRoleAsync(user, request.RoleName);
		}

		if (!result.Succeeded)
		{
			var errors = result.Errors.Select(e => e.Description).ToList();
			string messageErrors = string.Join(',', [.. errors]);
			return new ApiBadRequestResponse($"{messageErrors}. Failed to {actionVerb} role '{request.RoleName}' {preposition} user.");
		}

		return new ApiOkResponse<string>($"Role '{request.RoleName}' {actionVerb}ed {preposition} user successfully.");
	}
}