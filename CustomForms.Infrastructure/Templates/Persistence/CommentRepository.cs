using CustomForms.Application.Common.Interfaces;
using CustomForms.Domain.Common.RequestFeatures.ModelParameters;
using CustomForms.Domain.Common.RequestFeatures;
using CustomForms.Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;
using CustomForms.Infrastructure.Common.Persistence.Extensions;
using CustomForms.Domain.Templates;

namespace CustomForms.Infrastructure.Templates.Persistence;

public class CommentRepository(AppDbContext dbContext) : RepositoryBase<Comment>(dbContext), ICommentRepository
{
	public async Task<List<Comment>> FindByTemplateIdAsync(Guid templateId, bool trackChanges = false, CancellationToken cancellationToken = default)
	{
		return await FindByCondition(c => c.TemplateId == templateId, trackChanges)
			.Include(c => c.User) 
			.OrderBy(c => c.CreatedDate) 
			.ToListAsync(cancellationToken);
	}
}
