using CustomForms.Application.Common.DTOs;
using CustomForms.Application.Common.Responses;
using CustomForms.Application.UseCases.Comments.AddComment;
using CustomForms.Application.UseCases.Comments.DeleteComment;
using CustomForms.Domain.Errors;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CustomForms.API.Controllers;

[Route("api/comments")] 
public class CommentsController(ISender sender) : ApiControllerBase
{
	private readonly ISender _sender = sender;

	[HttpPost]
	[Authorize] 
	[ProducesResponseType(typeof(CommentDto), StatusCodes.Status201Created)]
	[ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status404NotFound)] 
	[ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status403Forbidden)] 
	public async Task<IActionResult> AddComment([FromBody] AddCommentDto commentData)
	{
		var result = await _sender.Send(new AddCommentUseCase(commentData));
		if (result.Success && result is ApiOkResponse<CommentDto> okResult)
		{
			return StatusCode(StatusCodes.Status201Created, okResult.Result);
		}
		return ProcessError(result);
	}

	[HttpDelete("{id:guid}")]
	[Authorize] 
	[ProducesResponseType(StatusCodes.Status200OK)] 
	[ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status403Forbidden)]
	public async Task<IActionResult> DeleteComment(Guid id)
	{
		var result = await _sender.Send(new DeleteCommentUseCase(id));
		return result.Success ? Ok() : ProcessError(result);
	}
}
