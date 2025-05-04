using CustomForms.Application.Common.Interfaces;
using CustomForms.Application.Common.Responses;
using MediatR;

namespace CustomForms.Application.UseCases.Forms.DeleteForm;

public class DeleteFormHandler(IRepositoryManager repManager, ICurrentUserService currentUserService) : IRequestHandler<DeleteFormUseCase, ApiBaseResponse>
{
	private readonly IRepositoryManager _repManager = repManager;
	private readonly ICurrentUserService _currentUserService = currentUserService;

	public async Task<ApiBaseResponse> Handle(DeleteFormUseCase request, CancellationToken cancellationToken)
	{
		var form = await _repManager.Forms.FindAsync(request.FormId);
		if (form == null)
		{
			return new ApiNotFoundResponse($"Form with ID '{request.FormId}' not found.");
		}

		var userId = _currentUserService.UserId;
		var isAdmin = await _currentUserService.IsAdminAsync();

		if (!isAdmin && form.UserId != userId)
		{
			return new ApiForbiddenResponse("You can only delete your own submitted forms.");
		}

		_repManager.Forms.Delete(form);
		await _repManager.CommitAsync(cancellationToken);

		return new ApiOkResponse<string>("Form deleted successfully.");
	}
}
