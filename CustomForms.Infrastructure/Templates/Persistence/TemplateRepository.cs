using CustomForms.Application.Common.Interfaces;
using CustomForms.Domain.Common.RequestFeatures.ModelParameters;
using CustomForms.Domain.Common.RequestFeatures;
using CustomForms.Domain.Forms;
using CustomForms.Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;
using CustomForms.Infrastructure.Common.Persistence.Extensions;
using System.Diagnostics;

namespace CustomForms.Infrastructure.Templates.Persistence;

public class TemplateRepository(AppDbContext dbContext) : RepositoryBase<Template>(dbContext), ITemplateRepository
{
	public async Task<PagedList<Template>> GetTemplatesAsync(Guid id, bool isAdmin, TemplateParameters templateParameters, CancellationToken cancellationToken = default)
	{
		var templatesQuery = FindAll(trackChanges: false) 
			.Include(t => t.Author) 
			.Include(t => t.Topic)
			.Include(t => t.Tags)
			.Include(t => t.Likes)
			.Include(t => t.Comments)
			.Include(t => t.Forms)
			
			.Where(t => isAdmin || t.IsPublic || t.AllowedUsers.Any(u => u.Id == id) || t.AuthorId == id)
			.FilterTemplates(templateParameters) 
			.Search(templateParameters.SearchTerm) 
			.Sort(templateParameters.OrderBy); 

		return await PagedList<Template>.ToPagedListAsync(templatesQuery,
			templateParameters.PageNumber,
			templateParameters.PageSize,
			cancellationToken);
	}
	public async Task<Template?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
	{
		return await FindByCondition(t => t.Id == id, trackChanges: false)
			.Include(t => t.Author)
			.Include(t => t.Topic)
			.Include(t => t.Questions.OrderBy(q => q.Order))
			.Include(t => t.Tags)
			.Include(t => t.Comments.OrderBy(c => c.CreatedDate)).ThenInclude(c => c.User)
			.Include(t => t.Likes)
			.Include(t => t.AllowedUsers)
			.FirstOrDefaultAsync(cancellationToken);
	}

	public async Task<List<Template>> GetLatestPublicAsync(int count, CancellationToken cancellationToken = default)
	{
		return await FindAll(trackChanges: false)
			.Where(t => t.IsPublic)
			.OrderByDescending(t => t.CreatedDate)
			.Take(count)
			.Include(t => t.Author)
			.Include(t => t.Likes)
			.Include(t => t.Comments)
			.Include(t => t.Topic)
			.Include(t => t.Forms)
			.ToListAsync(cancellationToken);
	}

	public async Task<List<Template>> GetTopPopularPublicAsync(int count, CancellationToken cancellationToken = default)
	{
		return await FindAll(trackChanges: false)
			.Where(t => t.IsPublic)
			.OrderByDescending(t => t.Forms.Count)
			.Take(count)
			.Include(t => t.Author)
			.Include(t => t.Likes)
			.Include(t => t.Comments)
			.Include(t => t.Topic)
			.Include(t => t.Forms)
			.ToListAsync(cancellationToken);
	}

	public async Task<bool> CanUserAccessTemplateAsync(Guid templateId, Guid userId, bool isUserAdmin, CancellationToken cancellationToken = default)
	{
		if (isUserAdmin) return true;
		if (await _dbSet.AnyAsync(t => t.Id == templateId && t.IsPublic, cancellationToken)) return true;
		if (await _dbSet.AnyAsync(t => t.Id == templateId && t.AuthorId == userId, cancellationToken)) return true;
		return await _dbSet
				.Where(t => t.Id == templateId)
				.SelectMany(t => t.AllowedUsers)
				.AnyAsync(u => u.Id == userId, cancellationToken);
	}
}
