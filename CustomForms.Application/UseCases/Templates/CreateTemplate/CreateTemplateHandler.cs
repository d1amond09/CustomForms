using CustomForms.Application.Common.Interfaces;
using CustomForms.Application.Common.Responses;
using Microsoft.EntityFrameworkCore;
using CustomForms.Domain.Forms;
using CustomForms.Domain.Users;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using CustomForms.Domain.Templates;

namespace CustomForms.Application.UseCases.Templates.CreateTemplate;

public class CreateTemplateHandler(IRepositoryManager repManager, ICurrentUserService currentUserService, ICloudinaryService cloudinaryService) : IRequestHandler<CreateTemplateUseCase, ApiBaseResponse>
{
	private readonly IRepositoryManager _repManager = repManager;
	private readonly ICurrentUserService _currentUserService = currentUserService;
	private readonly ICloudinaryService _cloudinaryService = cloudinaryService;

	public async Task<ApiBaseResponse> Handle(CreateTemplateUseCase request, CancellationToken cancellationToken)
	{
		var userId = _currentUserService.UserId;
		if (userId == null)
		{
			return new ApiForbiddenResponse("User must be authenticated to create templates.");
		}

		var topicExists = await _repManager.Topics.ExistsAsync(t => t.Id == request.TemplateData.TopicId, cancellationToken);
		if (!topicExists)
		{
			return new ApiBadRequestResponse($"Topic with ID '{request.TemplateData.TopicId}' not found.");
		}

		List<Tag> tags = [];
		if (request.TemplateData.Tags != null && request.TemplateData.Tags.Any())
		{
			tags = await _repManager.Tags.FindOrCreateTagsAsync(request.TemplateData.Tags, cancellationToken);
		}

		List<User> allowedUsers = [];
		if (!request.TemplateData.IsPublic)
		{
			if (request.TemplateData.AllowedUserIds == null || !request.TemplateData.AllowedUserIds.Any())
			{
				return new ApiBadRequestResponse("Allowed users must be specified for restricted templates.");
			}
			var fetchedUsers = await _repManager.Users.GetUsersAsQueryable() 
								   .Where(u => request.TemplateData.AllowedUserIds.Contains(u.Id))
								   .ToListAsync(cancellationToken);

			if (fetchedUsers.Count != request.TemplateData.AllowedUserIds.Distinct().Count())
			{
				return new ApiBadRequestResponse("One or more allowed user IDs are invalid.");
			}
			allowedUsers.AddRange(fetchedUsers);
		}

		IFormFile? imageFile = request.TemplateData.ImageFile;
		(string SecureUrl, string PublicId)? result = null;
		if (imageFile != null && imageFile.Length > 0)
		{
			try
			{
				result = await _cloudinaryService.UploadImageAsync(imageFile);
			}
			catch (Exception ex)
			{
				return new ApiBadRequestResponse($"Failed to upload image: {ex.Message}");
			}
		}

		if(result == null) 
			return new ApiBadRequestResponse($"Failed to upload image");

		var template = new Template(
			Guid.NewGuid(),
			request.TemplateData.Title,
			request.TemplateData.Description,
			userId.Value,
			request.TemplateData.TopicId,
			request.TemplateData.IsPublic,
			result.Value.SecureUrl,
			result.Value.PublicId
		);

		template.SetTags(tags);

		if (!request.TemplateData.IsPublic)
		{
			template.SetAccess(false, allowedUsers);
		}

		await _repManager.Templates.CreateAsync(template, cancellationToken);

		await _repManager.CommitAsync(cancellationToken);

		return new ApiOkResponse<Guid>(template.Id);
	}
}
