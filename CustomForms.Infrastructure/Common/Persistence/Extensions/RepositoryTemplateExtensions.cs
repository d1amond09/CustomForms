using CustomForms.Infrastructure.Common.Persistence.Extensions.Utility;
using CustomForms.Domain.Common.RequestFeatures.ModelParameters;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using CustomForms.Domain.Forms;

namespace CustomForms.Infrastructure.Common.Persistence.Extensions;

public static class RepositoryTemplateExtensions
{
	public static IQueryable<Template> FilterTemplates(this IQueryable<Template> templates, TemplateParameters parameters)
	{
		if (parameters.AuthorId.HasValue)
		{
			templates = templates.Where(t => t.AuthorId == parameters.AuthorId.Value);
		}
		if (parameters.TopicId.HasValue)
		{
			templates = templates.Where(t => t.TopicId == parameters.TopicId.Value);
		}
		if (parameters.TagId.HasValue) 
		{
			templates = templates.Where(t => t.Tags.Any(tag => tag.Id == parameters.TagId.Value));
		}

		return templates;
	}

	public static IQueryable<Template> Search(this IQueryable<Template> templates, string? searchTerm)
	{
		if (string.IsNullOrWhiteSpace(searchTerm))
			return templates;

		var lowerCaseTerm = searchTerm.Trim().ToLower();

		return templates.Where(t => 
			EF.Functions.Like(t.Title.ToLower(), $"%{lowerCaseTerm}%") ||
			(t.Description != null && EF.Functions.Like(t.Description.ToLower(), $"%{lowerCaseTerm}%")) ||
			t.Tags.Any(tag => EF.Functions.Like(tag.Name.ToLower(), $"%{lowerCaseTerm}%")) ||
			(t.Author.UserName != null && EF.Functions.Like(t.Author.UserName.ToLower(), $"%{lowerCaseTerm}%")) 
		);
	}

	public static IQueryable<Template> Sort(this IQueryable<Template> templates, string? orderByQueryString)
	{
		if (string.IsNullOrWhiteSpace(orderByQueryString))
			return templates.OrderByDescending(e => e.CreatedDate); 

		if (orderByQueryString.Equals("popularity desc", StringComparison.OrdinalIgnoreCase))
		{
			return templates.OrderByDescending(t => t.Forms.Count);
		}
		if (orderByQueryString.Equals("popularity asc", StringComparison.OrdinalIgnoreCase))
		{
			return templates.OrderBy(t => t.Forms.Count);
		}


		var orderQuery = OrderQueryBuilder.CreateOrderQuery<Template>(orderByQueryString);

		if (string.IsNullOrWhiteSpace(orderQuery))
			return templates.OrderByDescending(e => e.CreatedDate); 

		return templates.OrderBy(orderQuery);
	}
}