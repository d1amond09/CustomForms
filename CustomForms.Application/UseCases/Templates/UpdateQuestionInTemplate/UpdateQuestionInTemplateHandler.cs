using CustomForms.Application.Common.DTOs;
using CustomForms.Application.Common.Interfaces;
using CustomForms.Application.Common.Responses;
using CustomForms.Domain.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CustomForms.Application.UseCases.Templates.UpdateQuestionInTemplate;

public class UpdateQuestionInTemplateHandler(IRepositoryManager repManager, ICurrentUserService currentUserService) : IRequestHandler<UpdateQuestionInTemplateUseCase, ApiBaseResponse>
{
	private readonly IRepositoryManager _repManager = repManager;
	private readonly ICurrentUserService _currentUserService = currentUserService;

	public async Task<ApiBaseResponse> Handle(UpdateQuestionInTemplateUseCase request, CancellationToken cancellationToken)
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
			template.UpdateQuestion(request.QuestionId, request.QuestionData.Title, request.QuestionData.Description, request.QuestionData.ShowInResults);
		}
		catch (EntityNotFoundException ex) 
		{
			return new ApiNotFoundResponse(ex.Message);
		}
		catch (Exception ex)
		{
			return new ApiBadRequestResponse(ex.Message);
		}

		await _repManager.CommitAsync(cancellationToken);

		return new ApiOkResponse<string>("Question updated successfully.");
	}
}