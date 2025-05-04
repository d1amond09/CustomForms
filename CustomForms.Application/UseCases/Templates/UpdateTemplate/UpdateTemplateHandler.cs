using CustomForms.Application.Common.DTOs;
using CustomForms.Application.Common.Interfaces;
using CustomForms.Application.Common.Responses;
using MediatR;

namespace CustomForms.Application.UseCases.Templates.UpdateTemplate;

public class UpdateTemplateHandler(IRepositoryManager repManager, ICurrentUserService currentUserService /*, ICloudStorageService cloudStorage*/) : IRequestHandler<UpdateTemplateUseCase, ApiBaseResponse>
{
	private readonly IRepositoryManager _repManager = repManager;
	private readonly ICurrentUserService _currentUserService = currentUserService;

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

		
		string? finalImageUrl = request.TemplateData.ImageUrl; 


		template.UpdateDetails(
			request.TemplateData.Title,
			request.TemplateData.Description,
			request.TemplateData.TopicId,
			finalImageUrl
		);

		await _repManager.CommitAsync(cancellationToken);

		return new ApiOkResponse<string>("Template updated successfully.");
	}
}