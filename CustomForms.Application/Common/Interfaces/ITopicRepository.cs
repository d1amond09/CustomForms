using CustomForms.Domain.Common.RequestFeatures.ModelParameters;
using CustomForms.Domain.Common.RequestFeatures;
using CustomForms.Domain.Forms;

namespace CustomForms.Application.Common.Interfaces;

public interface ITopicRepository : IRepositoryBase<Topic>
{
	Task<PagedList<Topic>> GetTopicsAsync(TopicParameters topicParameters, CancellationToken cancellationToken = default);
	Task<Topic?> FindByNameAsync(string name, bool trackChanges = false, CancellationToken cancellationToken = default);
	Task<Guid?> GetTopicIdByNameAsync(string name, CancellationToken cancellationToken = default);
}
