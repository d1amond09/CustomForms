using CustomForms.Domain.Forms;

namespace CustomForms.Application.Common.Interfaces;

public interface IQuestionRepository : IRepositoryBase<Question>
{
	Task<List<Question>> GetQuestionsByTemplateIdAsync(Guid templateId, bool trackChanges = false, CancellationToken cancellationToken = default);
}