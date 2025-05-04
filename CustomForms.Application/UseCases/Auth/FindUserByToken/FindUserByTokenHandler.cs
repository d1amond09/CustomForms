using CustomForms.Application.Common.Interfaces;
using CustomForms.Application.Common.Responses;
using Microsoft.AspNetCore.Identity;
using CustomForms.Domain.Users;
using MediatR;

namespace CustomForms.Application.UseCases.Auth.FindUserByToken;

public class FindUserByTokenHandler(ITokenValidator tokenValidator, UserManager<User> userManager) :
	IRequestHandler<FindUserByTokenUseCase, ApiBaseResponse>
{
	private readonly ITokenValidator _tokenValidator = tokenValidator;
	private readonly UserManager<User> _userManager = userManager;

	public async Task<ApiBaseResponse> Handle(FindUserByTokenUseCase request, CancellationToken cancellationToken)
	{
		var principal = _tokenValidator.GetPrincipalFromExpiredToken(request.TokenDto.AccessToken);
		var user = await _userManager.FindByNameAsync(principal.Identity?.Name!);

		if (user == null ||
			user.RefreshToken != request.TokenDto.RefreshToken ||
			user.RefreshTokenExpiryTime <= DateTime.Now)
			return new ApiBadRequestResponse("");

		return new ApiOkResponse<User>(user);
	}
}