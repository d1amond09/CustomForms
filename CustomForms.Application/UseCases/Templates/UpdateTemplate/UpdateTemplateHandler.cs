using CustomForms.Application.Common.DTOs;
using CustomForms.Application.Common.Interfaces;
using CustomForms.Application.Common.Responses;
using MediatR;

namespace CustomForms.Application.UseCases.Templates.UpdateTemplate;

public class UpdateTemplateHandler(IRepositoryManager repManager, ICurrentUserService currentUserService, ICloudinaryService cloudinaryService) 
	: IRequestHandler<UpdateTemplateUseCase, ApiBaseResponse>
{
	private readonly IRepositoryManager _repManager = repManager;
	private readonly ICurrentUserService _currentUserService = currentUserService;
	private readonly ICloudinaryService _cloudinaryService = cloudinaryService;

	public async Task<ApiBaseResponse> Handle(UpdateTemplateUseCase request, CancellationToken cancellationToken)
	{
		var template = await _repManager.Templates.FindAsync(request.TemplateId);
		if (template == null)
		{
			return new ApiNotFoundResponse($"Template with ID '{request.TemplateId}' not found.");
		}

		var userId = _currentUserService.UserId;
		var isAdmin = await _currentUserService.IsAdminAsync();

		if (!isAdmin && template.AuthorId != userId)
		{
			return new ApiForbiddenResponse("You can only update your own templates.");
		}

		if (template.TopicId != request.TemplateData.TopicId) 
		{
			var topicExists = await _repManager.Topics.ExistsAsync(t => t.Id == request.TemplateData.TopicId, cancellationToken);
			if (!topicExists)
			{
				return new ApiBadRequestResponse($"Topic with ID '{request.TemplateData.TopicId}' not found.");
			}
		}

		template.UpdateDetails(
			request.TemplateData.Title,
			request.TemplateData.Description,
			request.TemplateData.TopicId
		);

		string? oldImagePublicId = template.ImagePublicId;
		bool imageChanged = false;

		if (request.TemplateData.RemoveCurrentImage)
		{
			if (!string.IsNullOrEmpty(oldImagePublicId))
			{
				await _cloudinaryService.DeleteImageAsync(oldImagePublicId);
			}
			template.SetImage(null, null);
			imageChanged = true;
		}
		else if (request.TemplateData.NewImageFile != null && request.TemplateData.NewImageFile.Length > 0)
		{
			if (!string.IsNullOrEmpty(oldImagePublicId))
			{
				await _cloudinaryService.DeleteImageAsync(oldImagePublicId);
			}

			var uploadResult = await _cloudinaryService.UploadImageAsync(request.TemplateData.NewImageFile);
			if (uploadResult.HasValue)
			{
				template.SetImage(uploadResult.Value.SecureUrl, uploadResult.Value.PublicId);
			}
			else
			{
				Console.WriteLine($"Warning: Failed to upload new image for template {template.Id}, but other details will be updated.");
			}
			imageChanged = true;
		}


		await _repManager.CommitAsync(cancellationToken);

		return new ApiOkResponse<string>("Template updated successfully.");
	}
}