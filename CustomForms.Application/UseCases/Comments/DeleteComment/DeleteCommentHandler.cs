using CustomForms.Application.Common.Interfaces;
using CustomForms.Application.Common.Responses;
using MediatR;

namespace CustomForms.Application.UseCases.Comments.DeleteComment;

public class DeleteCommentHandler(IRepositoryManager repManager, ICurrentUserService currentUserService) : IRequestHandler<DeleteCommentUseCase, ApiBaseResponse>
{
	private readonly IRepositoryManager _repManager = repManager;
	private readonly ICurrentUserService _currentUserService = currentUserService;

	public async Task<ApiBaseResponse> Handle(DeleteCommentUseCase request, CancellationToken cancellationToken)
	{
		var userId = _currentUserService.UserId;
		if (userId == null)
		{
			return new ApiForbiddenResponse("User must be authenticated to delete comments.");
		}

		var comment = await _repManager.Comments.FindAsync([request.CommentId], cancellationToken);
		if (comment == null)
		{
			return new ApiNotFoundResponse($"Comment with ID '{request.CommentId}' not found.");
		}

		var isAdmin = await _currentUserService.IsAdminAsync();

		if (!isAdmin && comment.UserId != userId)
		{
			return new ApiForbiddenResponse("You can only delete your own comments.");
		}

		_repManager.Comments.Delete(comment);
		await _repManager.CommitAsync(cancellationToken);

		return new ApiOkResponse<string>("Comment deleted successfully.");
	}
}