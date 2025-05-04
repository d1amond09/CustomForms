using CustomForms.Application.Common.DTOs;
using CustomForms.Application.Common.Interfaces;
using CustomForms.Application.Common.Responses;
using CustomForms.Domain.Forms;
using MediatR;

namespace CustomForms.Application.UseCases.Likes.ToggleLike;

public class ToggleLikeHandler(IRepositoryManager repManager, ICurrentUserService currentUserService) : IRequestHandler<ToggleLikeUseCase, ApiBaseResponse>
{
	private readonly IRepositoryManager _repManager = repManager;
	private readonly ICurrentUserService _currentUserService = currentUserService;

	public async Task<ApiBaseResponse> Handle(ToggleLikeUseCase request, CancellationToken cancellationToken)
	{
		var userId = _currentUserService.UserId;
		if (userId == null)
		{
			return new ApiForbiddenResponse("User must be authenticated to like templates.");
		}

		var templateExists = await _repManager.Templates.ExistsAsync(t => t.Id == request.TemplateId, cancellationToken);
		if (!templateExists)
		{
			return new ApiNotFoundResponse($"Template with ID '{request.TemplateId}' not found.");
		}


		var existingLike = await _repManager.Likes.FindByUserAndTemplateAsync(userId.Value, request.TemplateId, trackChanges: true, cancellationToken); // Track changes for delete

		if (existingLike != null)
		{
			_repManager.Likes.Delete(existingLike);
		}
		else
		{
			var newLike = new Like(Guid.NewGuid(), request.TemplateId, userId.Value);
			await _repManager.Likes.CreateAsync(newLike, cancellationToken);
		}

		await _repManager.CommitAsync(cancellationToken);

		var likeCount = await _repManager.Likes.GetLikeCountForTemplateAsync(request.TemplateId, cancellationToken);
		var isLiked = existingLike == null;

		return new ApiOkResponse<ToggleLikeDto>(new ToggleLikeDto(likeCount, isLiked));
	}
}
