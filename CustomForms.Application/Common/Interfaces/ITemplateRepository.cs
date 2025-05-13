using CustomForms.Domain.Common.RequestFeatures.ModelParameters;
using CustomForms.Domain.Common.RequestFeatures;
using CustomForms.Domain.Forms;
using CustomForms.Domain.Templates;

namespace CustomForms.Application.Common.Interfaces;

public interface ITemplateRepository : IRepositoryBase<Template>
{
	Task<Template?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
	Task<PagedList<Template>> GetTemplatesAsync(Guid id, bool isAdmin, TemplateParameters templateParameters, CancellationToken cancellationToken = default); 
	Task<List<Template>> GetLatestPublicAsync(int count, CancellationToken cancellationToken = default);
	Task<List<Template>> GetTopPopularPublicAsync(int count, CancellationToken cancellationToken = default); 
	Task<bool> CanUserAccessTemplateAsync(Guid templateId, Guid userId, bool isUserAdmin, CancellationToken cancellationToken = default);
}
