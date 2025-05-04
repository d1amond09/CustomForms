using CustomForms.Domain.Common.RequestFeatures.ModelParameters;
using CustomForms.Domain.Common.RequestFeatures;
using CustomForms.Domain.Forms;

namespace CustomForms.Application.Common.Interfaces;

public interface IFormRepository : IRepositoryBase<Form>
{
	Task<Form?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
	Task<PagedList<Form>> GetFormsAsync(FormParameters formParameters, CancellationToken cancellationToken = default);
	Task<int> GetFormCountForTemplateAsync(Guid templateId, CancellationToken cancellationToken = default);
	Task<List<Answer>> GetAnswersForFormAsync(Guid formId, CancellationToken cancellationToken = default);
}
