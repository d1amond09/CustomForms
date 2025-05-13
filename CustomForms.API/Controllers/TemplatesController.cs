using CustomForms.Application.Common.DTOs;
using CustomForms.Application.Common.Responses;
using CustomForms.Domain.Common.RequestFeatures.ModelParameters;
using CustomForms.Domain.Common.RequestFeatures;
using CustomForms.Domain.Errors;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CustomForms.Application.UseCases.Templates.GetTemplates;
using CustomForms.Application.UseCases.Templates.GetTemplateById;
using CustomForms.Application.UseCases.Templates.GetPopularPublicTemplates;
using CustomForms.Application.UseCases.Templates.GetLatestPublicTemplates;
using CustomForms.Application.UseCases.Templates.CreateTemplate;
using CustomForms.Application.UseCases.Templates.UpdateTemplate;
using CustomForms.Application.UseCases.Templates.DeleteTemplate;
using CustomForms.Application.UseCases.Templates.AddQuestionToTemplate;
using CustomForms.Application.UseCases.Templates.RemoveQuestionFromTemplate;
using CustomForms.Application.UseCases.Templates.UpdateQuestionInTemplate;
using CustomForms.Application.UseCases.Templates.ReorderTemplateQuestions;
using CustomForms.Application.UseCases.Templates.SetTemplateTags;
using CustomForms.Application.UseCases.Templates.SetTemplateAccess;
using CustomForms.API.Extensions;
using CustomForms.Application.UseCases.Forms.GetForms;
using System.Text.Json;

namespace CustomForms.API.Controllers;

[Route("api/templates")]
[ApiController]
public class TemplatesController(ISender sender) : ApiControllerBase
{
	private readonly ISender _sender = sender;

	[HttpGet]
	[ProducesResponseType(typeof(PagedList<TemplateBriefDto>), StatusCodes.Status200OK)]
	public async Task<IActionResult> GetTemplates([FromQuery] TemplateParameters parameters)
	{
		var baseResult = await _sender.Send(new GetTemplatesUseCase(parameters));

		if (!baseResult.Success)
			return ProcessError(baseResult);

		var (formDtos, metaData) = baseResult.GetResult<(List<TemplateBriefDto>, MetaData)>();

		Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(metaData));

		return Ok(formDtos);
	}

	[HttpGet("{id:guid}", Name = "GetTemplateById")]
	[ProducesResponseType(typeof(TemplateDetailsDto), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status403Forbidden)]
	public async Task<IActionResult> GetTemplateById(Guid id)
	{
		var result = await _sender.Send(new GetTemplateByIdUseCase(id));
		if (result.Success && result is ApiOkResponse<TemplateDetailsDto> okResult)
		{
			return Ok(okResult.Result);
		}
		return ProcessError(result);
	}

	[HttpGet("latest")]
	[AllowAnonymous]
	[ProducesResponseType(typeof(List<TemplateBriefDto>), StatusCodes.Status200OK)]
	public async Task<IActionResult> GetLatestPublicTemplates([FromQuery] int count = 5)
	{
		var result = await _sender.Send(new GetLatestPublicTemplatesUseCase(count));
		if (result.Success && result is ApiOkResponse<List<TemplateBriefDto>> okResult)
		{
			return Ok(okResult.Result);
		}
		return ProcessError(result);
	}

	[HttpGet("popular")]
	[AllowAnonymous]
	[ProducesResponseType(typeof(List<TemplateBriefDto>), StatusCodes.Status200OK)]
	public async Task<IActionResult> GetPopularPublicTemplates([FromQuery] int count = 5)
	{
		var result = await _sender.Send(new GetPopularPublicTemplatesUseCase(count));
		if (result.Success && result is ApiOkResponse<List<TemplateBriefDto>> okResult)
		{
			return Ok(okResult.Result);
		}
		return ProcessError(result);
	}

	[HttpPost]
	[Authorize]
	[ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
	[ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status403Forbidden)]
	public async Task<IActionResult> CreateTemplate([FromForm] CreateTemplateDto templateData)
	{
		var result = await _sender.Send(new CreateTemplateUseCase(templateData));
		if (result.Success && result is ApiOkResponse<Guid> okResult)
		{
			return CreatedAtAction(nameof(GetTemplateById), new { id = okResult.Result }, okResult.Result);
		}
		return ProcessError(result);
	}

	[HttpPut("{id:guid}")]
	[Authorize]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status403Forbidden)]
	public async Task<IActionResult> UpdateTemplate(Guid id, [FromForm] UpdateTemplateDto templateData)
	{
		var result = await _sender.Send(new UpdateTemplateUseCase(id, templateData));
		return result.Success ? Ok() : ProcessError(result);
	}

	[HttpDelete("{id:guid}")]
	[Authorize]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status403Forbidden)]
	public async Task<IActionResult> DeleteTemplate(Guid id)
	{
		var result = await _sender.Send(new DeleteTemplateUseCase(id));
		return result.Success ? Ok() : ProcessError(result);
	}

	[HttpPost("{id:guid}/questions")]
	[Authorize]
	[ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
	[ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status403Forbidden)]
	public async Task<IActionResult> AddQuestionToTemplate(Guid id, [FromBody] CreateQuestionDto questionData)
	{
		var result = await _sender.Send(new AddQuestionToTemplateUseCase(id, questionData));
		if (result.Success && result is ApiOkResponse<Guid> okResult)
		{
			return StatusCode(StatusCodes.Status201Created, okResult.Result);
		}
		return ProcessError(result);
	}

	[HttpDelete("{id:guid}/questions/{questionId:guid}")]
	[Authorize]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status403Forbidden)]
	public async Task<IActionResult> RemoveQuestionFromTemplate(Guid id, Guid questionId)
	{
		var result = await _sender.Send(new RemoveQuestionFromTemplateUseCase(id, questionId));
		return result.Success ? Ok() : ProcessError(result);
	}

	[HttpPut("{id:guid}/questions/{questionId:guid}")]
	[Authorize]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status403Forbidden)]
	public async Task<IActionResult> UpdateQuestionInTemplate(Guid id, Guid questionId, [FromBody] UpdateQuestionDto questionData)
	{
		var result = await _sender.Send(new UpdateQuestionInTemplateUseCase(id, questionId, questionData));
		return result.Success ? Ok() : ProcessError(result);
	}

	[HttpPut("{id:guid}/questions/reorder")]
	[Authorize]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status403Forbidden)]
	public async Task<IActionResult> ReorderTemplateQuestions(Guid id, [FromBody] ReorderQuestionsDto reorderData)
	{
		var result = await _sender.Send(new ReorderTemplateQuestionsUseCase(id, reorderData));
		return result.Success ? Ok() : ProcessError(result);
	}

	[HttpPut("{id:guid}/tags")]
	[Authorize]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status403Forbidden)]
	public async Task<IActionResult> SetTemplateTags(Guid id, [FromBody] List<string> tagNames)
	{
		var result = await _sender.Send(new SetTemplateTagsUseCase(id, tagNames));
		return result.Success ? Ok() : ProcessError(result);
	}

	[HttpPut("{id:guid}/access")]
	[Authorize]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status403Forbidden)]
	public async Task<IActionResult> SetTemplateAccess(Guid id, [FromBody] SetTemplateAccessDto accessData)
	{
		var result = await _sender.Send(new SetTemplateAccessUseCase(id, accessData));
		return result.Success ? Ok() : ProcessError(result);
	}
}