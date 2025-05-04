using CustomForms.Application.Common.DTOs;
using CustomForms.Application.Common.Responses;
using MediatR;

namespace CustomForms.Application.UseCases.Forms.SubmitForm;

public sealed record SubmitFormUseCase(SubmitFormDto FormData) : IRequest<ApiBaseResponse>;