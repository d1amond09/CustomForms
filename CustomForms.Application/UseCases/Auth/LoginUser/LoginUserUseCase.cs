using CustomForms.Application.Common.Responses;
using CustomForms.Application.Common.DTOs;
using MediatR;

namespace CustomForms.Application.UseCases.Auth.LoginUser;

public sealed record LoginUserUseCase(UserForLoginDto UserToLogin) :
	IRequest<ApiBaseResponse>;