using CustomForms.Application.Common.Responses;
using Microsoft.AspNetCore.Identity;
using CustomForms.Domain.Users;
using AutoMapper;
using MediatR;

namespace CustomForms.Application.UseCases.Auth.RegisterUser;

public class RegisterUserHandler(UserManager<User> userManager, IMapper mapper) : IRequestHandler<RegisterUserUseCase, ApiBaseResponse>
{
	private readonly UserManager<User> _userManager = userManager;
	private readonly IMapper _mapper = mapper;

	public async Task<ApiBaseResponse> Handle(RegisterUserUseCase request, CancellationToken cancellationToken)
	{
		var user = _mapper.Map<User>(request.UserForRegistrationDto);

		var identityResult = await _userManager.CreateAsync(user, request.UserForRegistrationDto.Password);

		if (identityResult.Succeeded)
		{
			await _userManager.AddToRolesAsync(user, ["User"]);
		}

		(IdentityResult, User) result = new(identityResult, user);

		return new ApiOkResponse<(IdentityResult, User)>(result);
	}
}