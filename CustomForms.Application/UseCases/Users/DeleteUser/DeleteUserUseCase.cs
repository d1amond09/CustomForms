using CustomForms.Application.Common.Responses;
using MediatR;

namespace CustomForms.Application.UseCases.Users.DeleteUser;

public sealed record DeleteUserUseCase(Guid UserId) : IRequest<ApiBaseResponse>;
