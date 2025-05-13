using CustomForms.Application.Common.DTOs;
using CustomForms.Application.Common.Interfaces;
using CustomForms.Application.Common.Responses;
using CustomForms.Domain.Common.Constants;
using CustomForms.Domain.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CustomForms.Application.UseCases.Templates.ReorderTemplateQuestions;

public class ReorderTemplateQuestionsHandler(IRepositoryManager repManager, ICurrentUserService currentUserService) : IRequestHandler<ReorderTemplateQuestionsUseCase, ApiBaseResponse>
{
	private readonly IRepositoryManager _repManager = repManager;
	private readonly ICurrentUserService _currentUserService = currentUserService;

	public async Task<ApiBaseResponse> Handle(ReorderTemplateQuestionsUseCase request, CancellationToken cancellationToken)
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
		var isAdmin = await _currentUserService.IsAdminAsync();
		bool canUserManage = template.CanUserManage(user) || isAdmin;

		if (!canUserManage)
		{
			return new ApiForbiddenResponse("You do not have permission to modify this template.");
		}

		try
		{
			template.ReorderQuestions(request.ReorderData.OrderedQuestionIds); 
		}
		catch (InvalidQuestionOrderException ex)
		{
			return new ApiBadRequestResponse(ex.Message);
		}
		catch (Exception ex)
		{
			return new ApiBadRequestResponse(ex.Message);
		}

		await _repManager.CommitAsync(cancellationToken);

		return new ApiOkResponse<string>("Questions reordered successfully.");
	}
}