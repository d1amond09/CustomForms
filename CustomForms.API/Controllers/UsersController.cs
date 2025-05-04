using CustomForms.Application.Common.DTOs;
using CustomForms.Domain.Common.Constants;
using CustomForms.Domain.Common.RequestFeatures.ModelParameters;
using CustomForms.Domain.Common.RequestFeatures;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CustomForms.Application.UseCases.Users.DeleteUser;
using CustomForms.Application.UseCases.Users.SetUserRole;
using CustomForms.Application.UseCases.Users.UnblockUser;
using CustomForms.Application.UseCases.Users.BlockUser;
using CustomForms.Application.UseCases.Users.GetUsers;
using CustomForms.API.Extensions;
using CustomForms.Application.UseCases.Users.GetUserMe;
using MySqlX.XDevAPI.Common;
using System.Text.Json;

namespace CustomForms.API.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController(ISender sender) : ApiControllerBase
{
	private readonly ISender _sender = sender;

	[Authorize]
	[HttpGet("me")]
	public async Task<IActionResult> GetUserMe()
	{
		var result = await _sender.Send(new GetUserMeUseCase());
		return result.Success
			? Ok(result.GetResult<UserDetailsDto>())
			: ProcessError(result);
	}

	[HttpGet]
	public async Task<IActionResult> GetUsers([FromQuery] UserParameters userParameters)
	{
		var baseResult = await _sender.Send(new GetUsersUseCase(userParameters));
		
		if (!baseResult.Success)
			return ProcessError(baseResult);

		var (userDtos, metaData) = baseResult.GetResult<(List<UserDetailsDto>, MetaData)>();

		Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(metaData));

		return Ok(userDtos);
	}

	[HttpPut("{id:guid}/block")]
	[Authorize(Roles = Roles.Admin)]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public async Task<IActionResult> BlockUser(Guid id)
	{
		var result = await _sender.Send(new BlockUserUseCase(id));
		return result.Success ? Ok() : ProcessError(result); 
	}

	[HttpPut("{id:guid}/unblock")]
	[Authorize(Roles = Roles.Admin)]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public async Task<IActionResult> UnblockUser(Guid id)
	{
		var result = await _sender.Send(new UnblockUserUseCase(id));
		return result.Success ? Ok() : ProcessError(result);
	}

	[HttpDelete("{id:guid}")]
	[Authorize(Roles = Roles.Admin)]
	[ProducesResponseType(StatusCodes.Status200OK)] 
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)] 
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public async Task<IActionResult> DeleteUser(Guid id)
	{
		var result = await _sender.Send(new DeleteUserUseCase(id));
		return result.Success ? Ok() : ProcessError(result);
	}

	[HttpPost("set-role")]
	[Authorize(Roles = Roles.Admin)]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public async Task<IActionResult> SetUserRole([FromBody] SetUserRoleUseCase command) 
	{
		var result = await _sender.Send(command);
		return result.Success ? Ok() : ProcessError(result);
	}
}
