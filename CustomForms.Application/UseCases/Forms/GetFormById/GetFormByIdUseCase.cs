using CustomForms.Application.Common.Responses;
using MediatR;

namespace CustomForms.Application.UseCases.Forms.GetFormById;

public sealed record GetFormByIdUseCase(Guid FormId) : IRequest<ApiBaseResponse>;

