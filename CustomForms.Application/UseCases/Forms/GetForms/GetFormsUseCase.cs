using CustomForms.Application.Common.DTOs;
using CustomForms.Domain.Common.RequestFeatures.ModelParameters;
using CustomForms.Domain.Common.RequestFeatures;
using MediatR;
using CustomForms.Application.Common.Responses;

namespace CustomForms.Application.UseCases.Forms.GetForms;

public sealed record GetFormsUseCase(FormParameters Parameters) : IRequest<ApiBaseResponse>;
