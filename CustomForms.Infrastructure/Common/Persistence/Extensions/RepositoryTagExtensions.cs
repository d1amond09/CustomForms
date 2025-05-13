using CustomForms.Infrastructure.Common.Persistence.Extensions.Utility;
using CustomForms.Domain.Common.RequestFeatures.ModelParameters;
using System.Linq.Dynamic.Core;
using CustomForms.Domain.Forms;
using Microsoft.EntityFrameworkCore;
using CustomForms.Domain.Templates;

namespace CustomForms.Infrastructure.Common.Persistence.Extensions;

public static class RepositoryTagExtensions
{
	public static IQueryable<Tag> FilterTags(this IQueryable<Tag> tags, TagParameters parameters)
	{
		return tags;
	}

	public static IQueryable<Tag> Search(this IQueryable<Tag> tags, string? searchTerm)
	{
		if (string.IsNullOrWhiteSpace(searchTerm))
			return tags;

		var lowerCaseTerm = searchTerm.Trim().ToLower();

		return tags.Where(t => EF.Functions.Like(t.Name.ToLower(), $"%{lowerCaseTerm}%"));
	}

	public static IQueryable<Tag> Sort(this IQueryable<Tag> tags, string? orderByQueryString, bool? onlyPopular = null)
	{
		if (onlyPopular == true || orderByQueryString?.Equals("popularity desc", StringComparison.OrdinalIgnoreCase) == true)
		{
			return tags.OrderByDescending(t => t.Templates.Count());
		}
		if (orderByQueryString?.Equals("popularity asc", StringComparison.OrdinalIgnoreCase) == true)
		{
			return tags.OrderBy(t => t.Templates.Count());
		}


		if (string.IsNullOrWhiteSpace(orderByQueryString))
			return tags.OrderBy(t => t.Name); 

		var orderQuery = OrderQueryBuilder.CreateOrderQuery<Tag>(orderByQueryString);

		if (string.IsNullOrWhiteSpace(orderQuery))
			return tags.OrderBy(t => t.Name);

		return tags.OrderBy(orderQuery);
	}
}