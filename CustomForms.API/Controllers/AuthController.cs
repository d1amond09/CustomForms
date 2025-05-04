using CustomForms.Application.UseCases.Auth.RegisterUser;
using CustomForms.Application.UseCases.Auth.CreateToken;
using CustomForms.Application.UseCases.Auth.LoginUser;
using CustomForms.Application.Common.DTOs;
using Microsoft.AspNetCore.Identity;
using CustomForms.API.Extensions;
using Microsoft.AspNetCore.Mvc;
using CustomForms.Domain.Users;
using MediatR;

namespace CustomForms.API.Controllers;

[Consumes("application/json")]
[Route("api/auth")]
[ApiController]
public class AuthController(ISender sender, UserManager<User> userManager) : ApiControllerBase
{
	private readonly ISender _sender = sender;
	private readonly UserManager<User> _userManager = userManager;

	[HttpPost("register", Name = "SignUp")]
	public async Task<IActionResult> RegisterUser([FromBody] UserForRegistrationDto userForRegistration)
	{
		var baseResult = await _sender.Send(new RegisterUserUseCase(userForRegistration));

		var (result, user) = baseResult.GetResult<(IdentityResult, User)>();

		if (!result.Succeeded)
		{
			foreach (var error in result.Errors)
			{
				ModelState.TryAddModelError(error.Code, error.Description);
			}
			return BadRequest(ModelState);
		}

		return StatusCode(201);
	}

	[HttpPost("login", Name = "SignIn")]
	public async Task<IActionResult> LoginUser([FromBody] UserForLoginDto userForLogin)
	{
		var baseResult = await _sender.Send(new LoginUserUseCase(userForLogin));

		var (isValid, user) = baseResult.GetResult<(bool, User?)>();

		if (!isValid || user == null)
			return Unauthorized("Invalid username or password.");

		var tokenDtoBaseResult = await _sender.Send(new CreateTokenUseCase(user, PopulateExp: true));
		var tokenDto = tokenDtoBaseResult.GetResult<TokenDto>();

		return Ok(tokenDto);
	}
}

