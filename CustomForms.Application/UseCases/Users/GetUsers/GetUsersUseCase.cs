using CustomForms.Application.Common.DTOs;
using CustomForms.Domain.Common.RequestFeatures.ModelParameters;
using CustomForms.Domain.Common.RequestFeatures;
using MediatR;
using CustomForms.Application.Common.Responses;

namespace CustomForms.Application.UseCases.Users.GetUsers;

public sealed record GetUsersUseCase(UserParameters Parameters) : IRequest<ApiBaseResponse>;
