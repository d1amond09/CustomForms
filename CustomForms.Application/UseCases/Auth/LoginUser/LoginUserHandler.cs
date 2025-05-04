using CustomForms.Application.Common.Responses;
using Microsoft.AspNetCore.Identity;
using CustomForms.Domain.Users;
using MediatR;

namespace CustomForms.Application.UseCases.Auth.LoginUser;

public class LoginUserHandler(UserManager<User> userManager) : IRequestHandler<LoginUserUseCase, ApiBaseResponse>
{
	private readonly UserManager<User> _userManager = userManager;

	public async Task<ApiBaseResponse> Handle(LoginUserUseCase request, CancellationToken cancellationToken)
	{
		User? user = await _userManager.FindByNameAsync(request.UserToLogin.UserName ?? "");

		bool isValid = user != null &&
			await _userManager.CheckPasswordAsync(user, request.UserToLogin.Password ?? "");

		(bool, User) result = new(isValid, user);

		return new ApiOkResponse<(bool, User?)>(result);
	}
}