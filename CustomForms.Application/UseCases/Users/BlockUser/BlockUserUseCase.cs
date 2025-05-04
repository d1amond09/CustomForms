using CustomForms.Application.Common.Responses;
using MediatR;

namespace CustomForms.Application.UseCases.Users.BlockUser;

public sealed record BlockUserUseCase(Guid UserId) : IRequest<ApiBaseResponse>;
