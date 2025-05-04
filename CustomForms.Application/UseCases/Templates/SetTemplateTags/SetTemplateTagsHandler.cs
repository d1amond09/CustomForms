using CustomForms.Application.Common.Interfaces;
using CustomForms.Application.Common.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CustomForms.Application.UseCases.Templates.SetTemplateTags;

public class SetTemplateTagsHandler(IRepositoryManager repManager, ICurrentUserService currentUserService) : IRequestHandler<SetTemplateTagsUseCase, ApiBaseResponse>
{
	private readonly IRepositoryManager _repManager = repManager;
	private readonly ICurrentUserService _currentUserService = currentUserService;

	public async Task<ApiBaseResponse> Handle(SetTemplateTagsUseCase request, CancellationToken cancellationToken)
	{
		var template = await _repManager.Templates
		   .FindByCondition(t => t.Id == request.TemplateId, trackChanges: true)
		   .Include(t => t.Tags) 
		   .FirstOrDefaultAsync(cancellationToken);

		if (template == null)
		{
			return new ApiNotFoundResponse($"Template with ID '{request.TemplateId}' not found.");
		}

		var user = await _currentUserService.GetCurrentUserAsync(cancellationToken);

		if (!template.CanUserManage(user))
		{
			return new ApiForbiddenResponse("You do not have permission to modify this template.");
		}

		var tagsToSet = await _repManager.Tags.FindOrCreateTagsAsync(request.TagNames ?? new List<string>(), cancellationToken);

		template.SetTags(tagsToSet);

		await _repManager.CommitAsync(cancellationToken);

		return new ApiOkResponse<string>("Template tags updated successfully.");
	}
}