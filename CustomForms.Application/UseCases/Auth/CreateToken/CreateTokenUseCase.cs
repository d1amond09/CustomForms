using CustomForms.Application.Common.Responses;
using CustomForms.Domain.Users;
using MediatR;

namespace CustomForms.Application.UseCases.Auth.CreateToken;

public sealed record CreateTokenUseCase(User User, bool PopulateExp) :
	IRequest<ApiBaseResponse>;