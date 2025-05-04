using CustomForms.Application.Common.DTOs;
using CustomForms.Domain.Common.RequestFeatures;
using MediatR;
using CustomForms.Application.Common.Responses;
using AutoMapper;
using CustomForms.Application.Common.Interfaces;
using CustomForms.Domain.Users;
using Microsoft.AspNetCore.Identity;

namespace CustomForms.Application.UseCases.Users.GetUserMe;

public class GetUserMeHandler(IMapper mapper, ICurrentUserService currentUserService, UserManager<User> userManager) : IRequestHandler<GetUserMeUseCase, ApiBaseResponse>
{
	private readonly IMapper _mapper = mapper;
	private readonly ICurrentUserService _currentUserService = currentUserService;
	private readonly UserManager<User> _userManager = userManager; 

	public async Task<ApiBaseResponse> Handle(GetUserMeUseCase request, CancellationToken cancellationToken)
	{
		var user = await _currentUserService.GetCurrentUserAsync(cancellationToken);
		if (user == null)
		{
			return new ApiNotFoundResponse("Not found user");
		}

		var userDto = _mapper.Map<UserDetailsDto>(user);
		userDto.Roles = await _userManager.GetRolesAsync(user); 

		return new ApiOkResponse<UserDetailsDto>(userDto);
	}
}