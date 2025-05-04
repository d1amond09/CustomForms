using CustomForms.Infrastructure.Common.Persistence.Extensions.Utility;
using CustomForms.Domain.Common.RequestFeatures.ModelParameters;
using System.Linq.Dynamic.Core;
using CustomForms.Domain.Forms;

namespace CustomForms.Infrastructure.Common.Persistence.Extensions;

public static class RepositoryFormExtensions
{
	public static IQueryable<Form> FilterForms(this IQueryable<Form> forms, FormParameters parameters)
	{
		if (parameters.TemplateId.HasValue)
		{
			forms = forms.Where(f => f.TemplateId == parameters.TemplateId.Value);
		}
		if (parameters.UserId.HasValue)
		{
			forms = forms.Where(f => f.UserId == parameters.UserId.Value);
		}
		if (parameters.MinFilledDate.HasValue)
		{
			forms = forms.Where(f => f.FilledDate >= parameters.MinFilledDate.Value);
		}
		if (parameters.MaxFilledDate.HasValue)
		{
			forms = forms.Where(f => f.FilledDate <= parameters.MaxFilledDate.Value);
		}
		return forms;
	}

	public static IQueryable<Form> Sort(this IQueryable<Form> forms, string? orderByQueryString)
	{
		if (string.IsNullOrWhiteSpace(orderByQueryString))
			return forms.OrderByDescending(e => e.FilledDate); 

		var orderQuery = OrderQueryBuilder.CreateOrderQuery<Form>(orderByQueryString);

		if (string.IsNullOrWhiteSpace(orderQuery))
			return forms.OrderByDescending(e => e.FilledDate); 

		return forms.OrderBy(orderQuery);
	}
}