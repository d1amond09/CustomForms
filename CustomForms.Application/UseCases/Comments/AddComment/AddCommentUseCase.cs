using CustomForms.Application.Common.DTOs;
using CustomForms.Application.Common.Responses;
using MediatR;

namespace CustomForms.Application.UseCases.Comments.AddComment;

public sealed record AddCommentUseCase(AddCommentDto CommentData) : IRequest<ApiBaseResponse>;
