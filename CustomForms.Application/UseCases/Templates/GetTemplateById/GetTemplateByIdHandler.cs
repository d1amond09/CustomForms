using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CustomForms.Application.Common.DTOs;
using CustomForms.Application.Common.Interfaces;
using CustomForms.Application.Common.Responses;
using CustomForms.Domain.Forms;
using CustomForms.Domain.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CustomForms.Application.UseCases.Templates.GetTemplateById;

public class GetTemplateByIdHandler(IRepositoryManager repManager, IMapper mapper, ICurrentUserService currentUserService) : IRequestHandler<GetTemplateByIdUseCase, ApiBaseResponse>
{
	private readonly IRepositoryManager _repManager = repManager;
	private readonly IMapper _mapper = mapper;
	private readonly ICurrentUserService _currentUserService = currentUserService;

	public async Task<ApiBaseResponse> Handle(GetTemplateByIdUseCase request, CancellationToken cancellationToken)
	{
		var template = await _repManager.Templates.GetByIdWithDetailsAsync(request.TemplateId, cancellationToken);

		if (template == null)
		{
			return new ApiNotFoundResponse($"Template with ID '{request.TemplateId}' not found.");
		}

		var currentUser = await _currentUserService.GetCurrentUserAsync(cancellationToken);
		var isAdmin = currentUser != null && await _currentUserService.IsAdminAsync(); 

		bool canView;
		if (template.IsPublic)
		{
			canView = true;
		}
		else
		{
			canView = currentUser != null && (isAdmin || await _repManager.Templates.CanUserAccessTemplateAsync(template.Id, currentUser.Id, false , cancellationToken));
		}


		if (!canView)
		{
			return new ApiNotFoundResponse($"Template with ID '{request.TemplateId}' not found.");
		}


		var templateDto = _mapper.Map<TemplateDetailsDto>(template);

		templateDto.CanCurrentUserManage = currentUser != null 
			&& (isAdmin || template.AuthorId == currentUser.Id); 

		templateDto.CanCurrentUserFill = currentUser != null 
			&& !templateDto.CanCurrentUserManage 
			&& (isAdmin 
				|| template.IsPublic 
				|| template.AllowedUsers?.Any(u => u.Id == currentUser.Id) == true); 

		templateDto.LikedByCurrentUser = currentUser != null 
			&& template.Likes.Any(l => l.UserId == currentUser.Id);

		if (!template.IsPublic && template.AllowedUsers != null)
		{
			templateDto.AllowedUsers = _mapper.Map<ICollection<UserSummaryDto>>(template.AllowedUsers);
		}
		else
		{
			templateDto.AllowedUsers = null; 
		}

		templateDto.Comments = [.. template.Comments.Select(c =>
		{
			var commentDto = _mapper.Map<CommentDto>(c);
			commentDto.CanCurrentUserDelete = currentUser != null && (isAdmin || c.UserId == currentUser.Id);
			return commentDto;
		})];


		return new ApiOkResponse<TemplateDetailsDto>(templateDto);
	}
}
