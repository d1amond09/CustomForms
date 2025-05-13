using CustomForms.Application.Common.Interfaces;
using CustomForms.Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;
using CustomForms.Infrastructure.Common.Persistence.Extensions;
using CustomForms.Domain.Templates;

namespace CustomForms.Infrastructure.Templates.Persistence;

public class QuestionRepository(AppDbContext dbContext) : RepositoryBase<Question>(dbContext), IQuestionRepository
{
	public async Task<List<Question>> GetQuestionsByTemplateIdAsync(Guid templateId, bool trackChanges = false, CancellationToken cancellationToken = default) => 
		await FindByCondition(q => q.TemplateId == templateId, trackChanges)
					 .OrderBy(q => q.Order) 
					 .ToListAsync(cancellationToken);
}
