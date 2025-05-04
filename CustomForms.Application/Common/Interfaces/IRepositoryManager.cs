using Microsoft.EntityFrameworkCore;

namespace CustomForms.Application.Common.Interfaces;

public interface IRepositoryManager
{
	IUserRepository Users { get; }
	ITemplateRepository Templates { get; }
	IQuestionRepository Questions { get; }
	IFormRepository Forms { get; }
	ICommentRepository Comments { get; }
	ILikeRepository Likes { get; }
	ITagRepository Tags { get; }
	ITopicRepository Topics { get; }

	Task<int> CommitAsync(CancellationToken cancellationToken = default);
}
