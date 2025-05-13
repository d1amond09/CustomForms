using CustomForms.Application.Common.Interfaces;
using CustomForms.Application.Common.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CustomForms.Application.UseCases.Templates.DeleteTemplate;

public class DeleteTemplateHandler(IRepositoryManager repManager, ICurrentUserService currentUserService, ICloudinaryService cloudinaryService) : IRequestHandler<DeleteTemplateUseCase, ApiBaseResponse>
{
	private readonly IRepositoryManager _repManager = repManager;
	private readonly ICurrentUserService _currentUserService = currentUserService;
	private readonly ICloudinaryService _cloudinaryService = cloudinaryService;
	public async Task<ApiBaseResponse> Handle(DeleteTemplateUseCase request, CancellationToken cancellationToken)
	{
		var template = await _repManager.Templates
			.FindByCondition(t => t.Id == request.TemplateId, trackChanges: true)
			.Include(t => t.Questions)
			.Include(t => t.Comments) 
			.Include(t => t.Likes)   
			.FirstOrDefaultAsync(cancellationToken);

		if (template == null)
		{
			return new ApiNotFoundResponse($"Template with ID '{request.TemplateId}' not found.");
		}

		var userId = _currentUserService.UserId;
		var isAdmin = await _currentUserService.IsAdminAsync();

		if (!isAdmin && template.AuthorId != userId)
		{
			return new ApiForbiddenResponse("You can only delete your own templates.");
		}

		var formCount = await _repManager.Forms.GetFormCountForTemplateAsync(request.TemplateId, cancellationToken);
		if (formCount > 0)
		{
			return new ApiBadRequestResponse($"Cannot delete template: {formCount} form(s) have been submitted using this template.");
		}

		if (!string.IsNullOrEmpty(template.ImagePublicId))
		{
			bool deletedFromCloud = await _cloudinaryService.DeleteImageAsync(template.ImagePublicId);
			if (!deletedFromCloud)
			{
				Console.WriteLine($"Warning: Failed to delete image {template.ImagePublicId} from Cloudinary for template {template.Id}");
			}
		}

		_repManager.Templates.Delete(template);
		await _repManager.CommitAsync(cancellationToken);

		return new ApiOkResponse<string>("Template deleted successfully.");
	}
}