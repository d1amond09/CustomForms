using AutoMapper;
using CustomForms.Application.Common.DTOs;
using CustomForms.Application.Common.Interfaces;
using CustomForms.Application.Common.Responses;
using CustomForms.Domain.Forms;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CustomForms.Application.UseCases.Forms.SubmitForm;

public class SubmitFormHandler(IRepositoryManager repManager, ICurrentUserService currentUserService, IMapper mapper) : IRequestHandler<SubmitFormUseCase, ApiBaseResponse>
{
	private readonly IRepositoryManager _repManager = repManager;
	private readonly ICurrentUserService _currentUserService = currentUserService;
	private readonly IMapper _mapper = mapper;

	public async Task<ApiBaseResponse> Handle(SubmitFormUseCase request, CancellationToken cancellationToken)
	{
		var userId = _currentUserService.UserId;
		if (userId == null)
		{
			return new ApiForbiddenResponse("User must be authenticated to submit forms.");
		}
		var currentUser = await _currentUserService.GetCurrentUserAsync(cancellationToken); 
		if (currentUser == null || currentUser.IsBlocked) 
		{
			return new ApiForbiddenResponse("User account is blocked.");
		}


		var template = await _repManager.Templates
			.FindByCondition(t => t.Id == request.FormData.TemplateId, trackChanges: false)
			.Include(t => t.Questions) 
			.Include(t => t.AllowedUsers) 
			.FirstOrDefaultAsync(cancellationToken);

		if (template == null)
		{
			return new ApiNotFoundResponse($"Template with ID '{request.FormData.TemplateId}' not found.");
		}

		var isAdmin = await _currentUserService.IsAdminAsync(); 
		bool canFill = template.CanUserFill(currentUser); 

		if (!canFill && !isAdmin) 
		{
			return new ApiForbiddenResponse("You do not have permission to fill this form.");
		}

		var templateQuestionIds = template.Questions.Select(q => q.Id).ToHashSet();
		var submittedQuestionIds = request.FormData.Answers.Select(a => a.QuestionId).ToList();

		if (submittedQuestionIds.Distinct().Count() != submittedQuestionIds.Count)
		{
			return new ApiBadRequestResponse("Duplicate answers submitted for the same question.");
		}
		if (!submittedQuestionIds.All(id => templateQuestionIds.Contains(id)) || submittedQuestionIds.Count != templateQuestionIds.Count)
		{
			return new ApiBadRequestResponse("Submitted answers do not match the questions in the template.");
		}
		var formId = Guid.NewGuid();
		var answers = request.FormData.Answers.Select(aDto =>
			new Answer(Guid.NewGuid(), formId, aDto.QuestionId, aDto.Value ?? "") 
		).ToList();

		var form = new Form(formId, template.Id, userId.Value, answers);

		await _repManager.Forms.CreateAsync(form, cancellationToken); 
		await _repManager.CommitAsync(cancellationToken);

		return new ApiOkResponse<Guid>(form.Id);
	}
}