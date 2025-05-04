using CustomForms.Application.Common.Responses;
using MediatR;

namespace CustomForms.Application.UseCases.Forms.DeleteForm;

public sealed record DeleteFormUseCase(Guid FormId) : IRequest<ApiBaseResponse>;
