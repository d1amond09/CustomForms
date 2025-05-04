using CustomForms.Application.UseCases.Auth.FindUserByToken;
using CustomForms.Application.UseCases.Auth.CreateToken;
using CustomForms.Application.Common.DTOs;
using CustomForms.API.Extensions;
using Microsoft.AspNetCore.Mvc;
using CustomForms.Domain.Users;
using MediatR;

namespace CustomForms.API.Controllers;

[Consumes("application/json")]
[Route("api/token")]
[ApiController]
public class TokenController(ISender sender) : ApiControllerBase
{
	private readonly ISender _sender = sender;

	[HttpPost("refresh")]
	public async Task<IActionResult> Refresh([FromBody] TokenDto tokenDto)
	{
		var baseResultRefresh = await _sender.Send(new FindUserByTokenUseCase(tokenDto));
		var user = baseResultRefresh.GetResult<User>();

		var baseResult = await _sender.Send(new CreateTokenUseCase(user, PopulateExp: false));
		var tokenDtoToReturn = baseResult.GetResult<TokenDto>();

		return Ok(tokenDtoToReturn);
	}
}
