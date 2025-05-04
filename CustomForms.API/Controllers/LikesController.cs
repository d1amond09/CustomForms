using CustomForms.Application.Common.DTOs;
using CustomForms.Application.Common.Responses;
using CustomForms.Application.UseCases.Likes.ToggleLike;
using CustomForms.Domain.Errors;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CustomForms.API.Controllers;

[Route("api/likes")]
public class LikesController(ISender sender) : ApiControllerBase
{
	private readonly ISender _sender = sender;

	[HttpPost("toggle/{templateId:guid}")]
	[Authorize] 
	[ProducesResponseType(typeof(ToggleLikeDto), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status404NotFound)] 
	[ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status403Forbidden)]
	public async Task<IActionResult> ToggleLike(Guid templateId)
	{
		var result = await _sender.Send(new ToggleLikeUseCase(templateId));
		if (result.Success && result is ApiOkResponse<ToggleLikeDto> okResult)
		{
			return Ok(okResult.Result);
		}
		return ProcessError(result);
	}
}
