using CustomForms.Application.Common.DTOs;
using CustomForms.Application.Common.Interfaces;
using CustomForms.Application.Common.Responses;
using CustomForms.Domain.Common.Exceptions;
using CustomForms.Domain.Forms;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CustomForms.Application.UseCases.Templates.AddQuestionToTemplate;

public class AddQuestionToTemplateHandler(IRepositoryManager repManager, ICurrentUserService currentUserService) : IRequestHandler<AddQuestionToTemplateUseCase, ApiBaseResponse>
{
	private readonly IRepositoryManager _repManager = repManager;
	private readonly ICurrentUserService _currentUserService = currentUserService;

	public async Task<ApiBaseResponse> Handle(AddQuestionToTemplateUseCase request, CancellationToken cancellationToken)
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

		var newQuestion = new Question(
			Guid.NewGuid(),
			template.Id,
			request.QuestionData.Title,
			request.QuestionData.Description,
			request.QuestionData.Type,
			request.QuestionData.ShowInResults
		);
		
		
		try
		{
			await _repManager.Questions.CreateAsync(newQuestion, cancellationToken);
			template.AddQuestion(newQuestion);
		}
		catch (Exception ex)
		{
			return new ApiBadRequestResponse(ex.Message);
		}

		await _repManager.CommitAsync(cancellationToken);

		return new ApiOkResponse<Guid>(newQuestion.Id);
	}
}