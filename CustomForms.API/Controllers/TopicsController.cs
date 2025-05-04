using CustomForms.Application.Common.DTOs;
using CustomForms.Application.Common.Responses;
using CustomForms.Domain.Common.RequestFeatures.ModelParameters;
using CustomForms.Domain.Common.RequestFeatures;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CustomForms.Application.UseCases.Topics.GetTopics;

namespace CustomForms.API.Controllers;

[Route("api/topics")]
public class TopicsController(ISender sender) : ApiControllerBase
{
	private readonly ISender _sender = sender;

	[HttpGet]
	[AllowAnonymous] 
	[ProducesResponseType(typeof(PagedList<TopicDto>), StatusCodes.Status200OK)]
	public async Task<IActionResult> GetTopics([FromQuery] TopicParameters parameters)
	{
		var result = await _sender.Send(new GetTopicsUseCase(parameters));
		if (result.Success && result is ApiOkResponse<PagedList<TopicDto>> okResult)
		{
			return Ok(okResult.Result);
		}
		return ProcessError(result);
	}
}