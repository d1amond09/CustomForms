using CustomForms.Application.Common.Interfaces;
using CustomForms.Domain.Common.RequestFeatures.ModelParameters;
using CustomForms.Domain.Common.RequestFeatures;
using CustomForms.Domain.Forms;
using CustomForms.Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;
using CustomForms.Infrastructure.Common.Persistence.Extensions;

namespace CustomForms.Infrastructure.Forms.Persistence;

public class FormRepository(AppDbContext dbContext) : RepositoryBase<Form>(dbContext), IFormRepository
{
	public async Task<PagedList<Form>> GetFormsAsync(FormParameters formParameters, CancellationToken cancellationToken = default)
	{
		var formsQuery = FindAll(trackChanges: false) 
			.Include(f => f.User) 
			.Include(f => f.Template)
				.ThenInclude(t => t.Topic)
			.FilterForms(formParameters) 
										
			.Sort(formParameters.OrderBy); 

		return await PagedList<Form>.ToPagedListAsync(formsQuery,
			formParameters.PageNumber,
			formParameters.PageSize,
			cancellationToken);
	}

	public async Task<Form?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
	{
		return await FindByCondition(f => f.Id == id, trackChanges: false)
			.Include(f => f.User)
			.Include(f => f.Template).ThenInclude(t => t.Topic)
			.Include(f => f.Answers).ThenInclude(a => a.Question)
			.FirstOrDefaultAsync(cancellationToken);
	}

	public async Task<int> GetFormCountForTemplateAsync(Guid templateId, CancellationToken cancellationToken = default)
	{
		return await CountAsync(f => f.TemplateId == templateId, cancellationToken);
	}

	public async Task<List<Answer>> GetAnswersForFormAsync(Guid formId, CancellationToken cancellationToken = default)
	{
		return await _db.Answers
					   .Where(a => a.FormId == formId)
					   .Include(a => a.Question)
					   .AsNoTracking()
					   .ToListAsync(cancellationToken);
	}
}

