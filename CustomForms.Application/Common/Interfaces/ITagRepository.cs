using CustomForms.Domain.Common.RequestFeatures.ModelParameters;
using CustomForms.Domain.Common.RequestFeatures;
using CustomForms.Domain.Forms;

namespace CustomForms.Application.Common.Interfaces;

public interface ITagRepository : IRepositoryBase<Tag>
{
	Task<PagedList<Tag>> GetTagsAsync(TagParameters tagParameters, CancellationToken cancellationToken = default);
	Task<Tag?> FindByNameAsync(string name, bool trackChanges = false, CancellationToken cancellationToken = default);
	Task<List<Tag>> SearchByNameAsync(string namePrefix, int maxResults, CancellationToken cancellationToken = default);
	Task<List<Tag>> FindOrCreateTagsAsync(IEnumerable<string> tagNames, CancellationToken cancellationToken = default);
	Task<List<Tag>> GetPopularTagsAsync(int count, CancellationToken cancellationToken = default); 
}
