using AutoMapper;
using CustomForms.Application.Common.DTOs;
using CustomForms.Application.Common.Interfaces;
using CustomForms.Application.Common.Responses;
using CustomForms.Domain.Forms;
using CustomForms.Domain.Templates;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CustomForms.Application.UseCases.Comments.AddComment;

public class AddCommentHandler(IRepositoryManager repManager, ICurrentUserService currentUserService, IMapper mapper) : IRequestHandler<AddCommentUseCase, ApiBaseResponse>
{
	private readonly IRepositoryManager _repManager = repManager;
	private readonly ICurrentUserService _currentUserService = currentUserService;
	private readonly IMapper _mapper = mapper;

	public async Task<ApiBaseResponse> Handle(AddCommentUseCase request, CancellationToken cancellationToken)
	{
		var userId = _currentUserService.UserId;
		if (userId == null)
		{
			return new ApiForbiddenResponse("User must be authenticated to add comments.");
		}

		var templateExists = await _repManager.Templates.ExistsAsync(t => t.Id == request.CommentData.TemplateId, cancellationToken);
		if (!templateExists)
		{
			return new ApiNotFoundResponse($"Template with ID '{request.CommentData.TemplateId}' not found.");
		}

		var isAdmin = await _currentUserService.IsAdminAsync();
		var canAccess = await _repManager.Templates.CanUserAccessTemplateAsync(request.CommentData.TemplateId, userId.Value, isAdmin, cancellationToken);
		if (!canAccess)
		{
			return new ApiForbiddenResponse("You do not have permission to comment on this template.");
		}


		var comment = new Comment(
			Guid.NewGuid(),
			request.CommentData.TemplateId,
			userId.Value,
			request.CommentData.Text
		);

		await _repManager.Comments.CreateAsync(comment, cancellationToken);
		await _repManager.CommitAsync(cancellationToken);

		var createdComment = await _repManager.Comments
			.FindByCondition(c => c.Id == comment.Id, trackChanges: false)
			.Include(c => c.User) 
			.FirstOrDefaultAsync(cancellationToken);

		var commentDto = _mapper.Map<CommentDto>(createdComment);
		commentDto.CanCurrentUserDelete = true;

		return new ApiOkResponse<CommentDto>(commentDto);
	}
}