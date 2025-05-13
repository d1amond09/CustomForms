using CustomForms.Domain.Forms;
using CustomForms.Domain.Templates;

namespace CustomForms.Application.Common.Interfaces;

public interface ILikeRepository : IRepositoryBase<Like>
{
	Task<List<Like>> FindByTemplateIdAsync(Guid templateId, bool trackChanges = false, CancellationToken cancellationToken = default);
	Task<Like?> FindByUserAndTemplateAsync(Guid userId, Guid templateId, bool trackChanges = false, CancellationToken cancellationToken = default);
	Task<bool> ExistsAsync(Guid userId, Guid templateId, CancellationToken cancellationToken = default);
	Task<int> GetLikeCountForTemplateAsync(Guid templateId, CancellationToken cancellationToken = default);
}
