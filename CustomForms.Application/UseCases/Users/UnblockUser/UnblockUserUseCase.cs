using CustomForms.Application.Common.Responses;
using MediatR;

namespace CustomForms.Application.UseCases.Users.UnblockUser;

public sealed record UnblockUserUseCase(Guid UserId) : IRequest<ApiBaseResponse>;
