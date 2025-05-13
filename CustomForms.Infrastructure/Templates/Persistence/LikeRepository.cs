using CustomForms.Application.Common.Interfaces;
using CustomForms.Domain.Common.RequestFeatures.ModelParameters;
using CustomForms.Domain.Common.RequestFeatures;
using CustomForms.Domain.Forms;
using CustomForms.Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;
using CustomForms.Infrastructure.Common.Persistence.Extensions;
using CustomForms.Domain.Templates;

namespace CustomForms.Infrastructure.Templates.Persistence;

public class LikeRepository(AppDbContext dbContext) : RepositoryBase<Like>(dbContext), ILikeRepository
{
	public async Task<List<Like>> FindByTemplateIdAsync(Guid templateId, bool trackChanges = false, CancellationToken cancellationToken = default)
	{
		return await FindByCondition(l => l.TemplateId == templateId, trackChanges)
			.Include(l => l.User) 
			.ToListAsync(cancellationToken);
	}

	public async Task<Like?> FindByUserAndTemplateAsync(Guid userId, Guid templateId, bool trackChanges = false, CancellationToken cancellationToken = default)
	{
		return await FindByCondition(l => l.UserId == userId && l.TemplateId == templateId, trackChanges)
			.FirstOrDefaultAsync(cancellationToken);
	}

	public async Task<bool> ExistsAsync(Guid userId, Guid templateId, CancellationToken cancellationToken = default) => 
		await ExistsAsync(l => l.UserId == userId && l.TemplateId == templateId, cancellationToken);

	public async Task<int> GetLikeCountForTemplateAsync(Guid templateId, CancellationToken cancellationToken = default) => 
		await CountAsync(l => l.TemplateId == templateId, cancellationToken);
}
