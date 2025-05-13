using CustomForms.Application.Common.Interfaces;
using CustomForms.Domain.Common.RequestFeatures.ModelParameters;
using CustomForms.Domain.Common.RequestFeatures;
using CustomForms.Domain.Forms;
using CustomForms.Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;
using CustomForms.Infrastructure.Common.Persistence.Extensions;
using CustomForms.Domain.Templates;

namespace CustomForms.Infrastructure.Templates.Persistence;

public class TopicRepository(AppDbContext dbContext) : RepositoryBase<Topic>(dbContext), ITopicRepository
{
	public async Task<PagedList<Topic>> GetTopicsAsync(TopicParameters topicParameters, CancellationToken cancellationToken = default)
	{
		var topicsQuery = FindAll(trackChanges: false)
			.Search(topicParameters.SearchTerm)
			.Sort(topicParameters.OrderBy);

		return await PagedList<Topic>.ToPagedListAsync(topicsQuery,
			topicParameters.PageNumber,
			topicParameters.PageSize,
			cancellationToken);
	}
	public async Task<Topic?> FindByNameAsync(string name, bool trackChanges = false, CancellationToken cancellationToken = default)
	{
		var normalizedName = name.Trim().ToLowerInvariant();
		return await FindByCondition(t => t.Name.ToLower() == normalizedName, trackChanges)
						.FirstOrDefaultAsync(cancellationToken);
	}

	public async Task<Guid?> GetTopicIdByNameAsync(string name, CancellationToken cancellationToken = default)
	{
		var normalizedName = name.Trim().ToLowerInvariant();
		return await FindByCondition(t => t.Name.ToLower() == normalizedName, trackChanges: false)
					   .Select(t => (Guid?)t.Id)
					   .FirstOrDefaultAsync(cancellationToken);
	}
}
