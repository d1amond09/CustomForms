using CustomForms.Application.Common.Responses;
using MediatR;

namespace CustomForms.Application.UseCases.Users.SetUserRole;

public sealed record SetUserRoleUseCase(Guid UserId, string RoleName, bool AddRole) : IRequest<ApiBaseResponse>;