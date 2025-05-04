using CustomForms.Infrastructure.Common.Persistence.Extensions.Utility;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using CustomForms.Domain.Forms;

namespace CustomForms.Infrastructure.Common.Persistence.Extensions;

public static class RepositoryTopicExtensions
{
	public static IQueryable<Topic> Search(this IQueryable<Topic> topics, string? searchTerm)
	{
		if (string.IsNullOrWhiteSpace(searchTerm))
			return topics;

		var lowerCaseTerm = searchTerm.Trim().ToLower();

		return topics.Where(t => EF.Functions.Like(t.Name.ToLower(), $"%{lowerCaseTerm}%"));
	}

	public static IQueryable<Topic> Sort(this IQueryable<Topic> topics, string? orderByQueryString)
	{
		if (string.IsNullOrWhiteSpace(orderByQueryString))
			return topics.OrderBy(t => t.Name);

		var orderQuery = OrderQueryBuilder.CreateOrderQuery<Topic>(orderByQueryString);

		if (string.IsNullOrWhiteSpace(orderQuery))
			return topics.OrderBy(t => t.Name); 

		return topics.OrderBy(orderQuery);
	}
}