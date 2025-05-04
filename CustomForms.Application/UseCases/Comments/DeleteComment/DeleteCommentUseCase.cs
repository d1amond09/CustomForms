using CustomForms.Application.Common.Responses;
using MediatR;

namespace CustomForms.Application.UseCases.Comments.DeleteComment;

public sealed record DeleteCommentUseCase(Guid CommentId) : IRequest<ApiBaseResponse>;
