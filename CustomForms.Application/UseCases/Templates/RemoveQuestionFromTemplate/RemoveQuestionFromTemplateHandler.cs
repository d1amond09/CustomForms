using CustomForms.Application.Common.Interfaces;
using CustomForms.Application.Common.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CustomForms.Application.UseCases.Templates.RemoveQuestionFromTemplate;

public class RemoveQuestionFromTemplateHandler(IRepositoryManager repManager, ICurrentUserService currentUserService) : IRequestHandler<RemoveQuestionFromTemplateUseCase, ApiBaseResponse>
{
	private readonly IRepositoryManager _repManager = repManager;
	private readonly ICurrentUserService _currentUserService = currentUserService;

	public async Task<ApiBaseResponse> Handle(RemoveQuestionFromTemplateUseCase request, CancellationToken cancellationToken)
	{
		var template = await _repManager.Templates
			.FindByCondition(t => t.Id == request.TemplateId, trackChanges: true)
			.Include(t => t.Questions) 
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

		try
		{
			var question = await _repManager.Questions.FirstOrDefaultAsync(t => t.Id.Equals(request.QuestionId), cancellationToken: cancellationToken);
			ArgumentNullException.ThrowIfNull(question);
			template.RemoveQuestion(question.Id);
			_repManager.Questions.Delete(question);
		}
		catch (ArgumentNullException)
		{
			return new ApiNotFoundResponse($"Question with ID '{request.QuestionId}' not found.");
		}
		catch (Exception ex) 
		{
			return new ApiBadRequestResponse(ex.Message);
		}

		await _repManager.CommitAsync(cancellationToken);

		return new ApiOkResponse<string>("Question removed successfully.");
	}
}