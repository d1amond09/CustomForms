using CustomForms.Application.Common.Responses;
using MediatR;

namespace CustomForms.Application.UseCases.Users.GetUserMe;

public sealed record GetUserMeUseCase() : IRequest<ApiBaseResponse>;
