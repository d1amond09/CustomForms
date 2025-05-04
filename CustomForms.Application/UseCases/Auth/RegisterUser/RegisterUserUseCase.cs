using CustomForms.Application.Common.Responses;
using CustomForms.Application.Common.DTOs;
using MediatR;

namespace CustomForms.Application.UseCases.Auth.RegisterUser;

public sealed record RegisterUserUseCase(UserForRegistrationDto UserForRegistrationDto) :
	IRequest<ApiBaseResponse>;