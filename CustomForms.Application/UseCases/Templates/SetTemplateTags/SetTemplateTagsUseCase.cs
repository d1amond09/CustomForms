using CustomForms.Application.Common.Responses;
using MediatR;

namespace CustomForms.Application.UseCases.Templates.SetTemplateTags;

public sealed record SetTemplateTagsUseCase(Guid TemplateId, List<string> TagNames) : IRequest<ApiBaseResponse>;