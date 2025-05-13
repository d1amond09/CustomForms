using CustomForms.Domain.Templates;

namespace CustomForms.Application.Common.Interfaces;

public interface ICommentRepository : IRepositoryBase<Comment>
{
	Task<List<Comment>> FindByTemplateIdAsync(Guid templateId, bool trackChanges = false, CancellationToken cancellationToken = default);
}