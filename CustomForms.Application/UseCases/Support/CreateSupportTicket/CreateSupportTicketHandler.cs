using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CustomForms.Application.Common.DTOs;
using CustomForms.Application.Common.Interfaces;
using CustomForms.Application.Common.Responses;
using MediatR;
using Microsoft.Extensions.Options;

namespace CustomForms.Application.UseCases.Support.CreateSupportTicket;

public class CreateSupportTicketCommandHandler(
	ICurrentUserService currentUserService,
	ICloudStorageService cloudStorageService)
		: IRequestHandler<CreateSupportTicketUseCase, ApiBaseResponse>
{
	private readonly ICurrentUserService _currentUserService = currentUserService;
	private readonly ICloudStorageService _cloudStorageService = cloudStorageService;

	public async Task<ApiBaseResponse> Handle(
		CreateSupportTicketUseCase request,
		CancellationToken cancellationToken)
	{
		var userId = _currentUserService.UserId;
		if (userId == null || string.IsNullOrEmpty(_currentUserService.UserName))
		{
			return new ApiForbiddenResponse("User not authenticated.");
		}

		var adminEmails = _cloudStorageService.GetAdminEmails().ToList();

		if (!adminEmails.Any())
		{
			Console.WriteLine("Warning: No admin emails configured in CloudStorageSettings for support ticket notifications.");
		}

		var ticketJson = new SupportTicketJsonDto
		{
			ReportedBy = _currentUserService.UserName!,
			UserEmail = _currentUserService.UserEmail ?? "N/A",
			Template = request.TicketData.TemplateTitle,
			Link = request.TicketData.PageUrl,
			Priority = request.TicketData.Priority,
			Summary = request.TicketData.Summary,
			ReportedAtUtc = DateTime.UtcNow,
			NotifyAdmins = adminEmails
		};

		string jsonContent = JsonSerializer.Serialize(ticketJson, new JsonSerializerOptions { WriteIndented = true });
		string fileName = $"SupportTicket_{DateTime.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid().ToString().Substring(0, 8)}.json";

		var uploadResult = await _cloudStorageService.UploadJsonFileAsync(fileName, jsonContent, cancellationToken);

		if (!uploadResult.Success)
		{
			return new ApiBadRequestResponse(uploadResult.ErrorMessage ?? "Failed to upload support ticket.");
		}

		return new ApiOkResponse<CloudUploadResult>(uploadResult);
	}
}