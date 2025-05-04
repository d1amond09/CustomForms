using CustomForms.Application.Common.Responses;
using CustomForms.Application.Common.DTOs;
using MediatR;

namespace CustomForms.Application.UseCases.Auth.FindUserByToken;

public sealed record FindUserByTokenUseCase(TokenDto TokenDto) :
	IRequest<ApiBaseResponse>;