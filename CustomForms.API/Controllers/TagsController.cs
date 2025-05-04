using CustomForms.Application.Common.DTOs;
using CustomForms.Application.Common.Responses;
using CustomForms.Domain.Common.RequestFeatures.ModelParameters;
using CustomForms.Domain.Common.RequestFeatures;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CustomForms.Application.UseCases.Tags.GetTags;
using CustomForms.Application.UseCases.Tags.GetPopularTags;
using MySqlX.XDevAPI.Common;
using System.Text.Json;
using CustomForms.API.Extensions;

namespace CustomForms.API.Controllers;

[Route("api/tags")]
public class TagsController(ISender sender) : ApiControllerBase
{
	private readonly ISender _sender = sender;

	[HttpGet]
	[AllowAnonymous] 
	[ProducesResponseType(typeof(PagedList<TagDto>), StatusCodes.Status200OK)]
	public async Task<IActionResult> GetTags([FromQuery] TagParameters parameters)
	{
		var baseResult = await _sender.Send(new GetTagsUseCase(parameters));

		if (!baseResult.Success)
			return ProcessError(baseResult);

		var (tagDtos, metaData) = baseResult.GetResult<(List<TemplateBriefDto>, MetaData)>();

		Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(metaData));

		return Ok(tagDtos);
	}

	[HttpGet("popular")]
	[AllowAnonymous] 
	[ProducesResponseType(typeof(List<TagDto>), StatusCodes.Status200OK)]
	public async Task<IActionResult> GetPopularTags([FromQuery] int count = 20)
	{
		var result = await _sender.Send(new GetPopularTagsUseCase(count));
		if (result.Success && result is ApiOkResponse<List<TagDto>> okResult)
		{
			return Ok(okResult.Result);
		}
		return ProcessError(result);
	}
}
