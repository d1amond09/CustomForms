using System.Text;
using CustomForms.API.Extensions;
using CustomForms.Application.Common.DTOs;
using CustomForms.Application.Common.Responses;
using CustomForms.Application.UseCases.Likes.ToggleLike;
using CustomForms.Application.UseCases.Support.CreateSupportTicket;
using CustomForms.Domain.Errors;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CustomForms.API.Controllers;

[Route("api/support")]
[Authorize] // User must be authenticated to create a ticket
public class SupportController(ISender sender) : ApiControllerBase
{
	private readonly ISender _sender = sender;

	[HttpPost("ticket")]
	[ProducesResponseType(typeof(CloudUploadResult), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status401Unauthorized)]
	public async Task<IActionResult> CreateTicket([FromBody] CreateSupportTicketDto ticketData)
	{
		string rawRequestBody = "";
		Request.EnableBuffering(); 
		using (var reader = new StreamReader(Request.Body, Encoding.UTF8, true, 1024, true))
		{
		    rawRequestBody = await reader.ReadToEndAsync();
		}
		Request.Body.Position = 0;
		Console.WriteLine("RAW REQUEST BODY FOR TICKET: " + rawRequestBody);
		var result = await _sender.Send(new CreateSupportTicketUseCase(ticketData));

		if (!result.Success)
		{
			return ProcessError(result);
		}
		return Ok(result.GetResult<CloudUploadResult>());
	}
}
