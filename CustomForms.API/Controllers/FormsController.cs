using CustomForms.Application.Common.DTOs;
using CustomForms.Application.Common.Responses;
using CustomForms.Domain.Common.RequestFeatures.ModelParameters;
using CustomForms.Domain.Common.RequestFeatures;
using CustomForms.Domain.Errors;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CustomForms.Application.UseCases.Forms.DeleteForm;
using CustomForms.Application.UseCases.Forms.SubmitForm;
using CustomForms.Application.UseCases.Forms.GetFormById;
using CustomForms.Application.UseCases.Forms.GetForms;
using System.Text.Json;
using MySqlX.XDevAPI.Common;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using CustomForms.API.Extensions;

namespace CustomForms.API.Controllers;

[Route("api/forms")]
public class FormsController(ISender sender) : ApiControllerBase
{
	private readonly ISender _sender = sender;

	[HttpGet]
	[Authorize] 
	[ProducesResponseType(typeof(PagedList<FormBriefDto>), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status403Forbidden)]
	public async Task<IActionResult> GetForms([FromQuery] FormParameters parameters)
	{
		var baseResult = await _sender.Send(new GetFormsUseCase(parameters));

		if (!baseResult.Success)
			return ProcessError(baseResult);

		var (formDtos, metaData) = baseResult.GetResult<(List<FormBriefDto>, MetaData)>();

		Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(metaData));
		
		return Ok(formDtos);
	}

	[HttpGet("{id:guid}", Name = "GetFormById")]
	[Authorize]
	[ProducesResponseType(typeof(FormDetailsDto), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status403Forbidden)]
	public async Task<IActionResult> GetFormById(Guid id)
	{
		var result = await _sender.Send(new GetFormByIdUseCase(id));
		if (result.Success && result is ApiOkResponse<FormDetailsDto> okResult)
		{
			return Ok(okResult.Result);
		}
		return ProcessError(result);
	}

	[HttpPost]
	[Authorize] 
	[ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
	[ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status404NotFound)] 
	[ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status403Forbidden)] 
	public async Task<IActionResult> SubmitForm([FromBody] SubmitFormDto formData)
	{
		var result = await _sender.Send(new SubmitFormUseCase(formData));
		if (result.Success && result is ApiOkResponse<Guid> okResult)
		{
			return CreatedAtAction(nameof(GetFormById), new { id = okResult.Result }, okResult.Result);
		}
		return ProcessError(result);
	}

	[HttpDelete("{id:guid}")]
	[Authorize] 
	[ProducesResponseType(StatusCodes.Status200OK)] 
	[ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status403Forbidden)]
	public async Task<IActionResult> DeleteForm(Guid id)
	{
		var result = await _sender.Send(new DeleteFormUseCase(id));
		return result.Success ? Ok() : ProcessError(result);
	}
}
