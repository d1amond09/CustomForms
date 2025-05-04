using CustomForms.Infrastructure.Common.Persistence.Extensions.Utility;
using CustomForms.Domain.Common.RequestFeatures.ModelParameters;
using System.Linq.Dynamic.Core;
using CustomForms.Domain.Users;

namespace CustomForms.Infrastructure.Common.Persistence.Extensions;

public static class RepositoryUserExtensions
{
	public static IQueryable<User> FilterUsers(this IQueryable<User> users, UserParameters parameters)
	{
		if (parameters.IsBlocked.HasValue)
		{
			users = users.Where(u => u.IsBlocked == parameters.IsBlocked.Value);
		}

		return users; 
	}

	public static IQueryable<User> Search(this IQueryable<User> users, string? searchTerm)
	{
		if (string.IsNullOrWhiteSpace(searchTerm))
			return users;

		var lowerCaseTerm = searchTerm.Trim().ToLower();

		return users.Where(u => 
			(u.FirstName != null && u.FirstName.ToLower().Contains(lowerCaseTerm)) ||
			(u.LastName != null && u.LastName.ToLower().Contains(lowerCaseTerm)) ||
			(u.UserName != null && u.UserName.ToLower().Contains(lowerCaseTerm)) ||
			(u.Email != null && u.Email.ToLower().Contains(lowerCaseTerm)));
	}

	public static IQueryable<User> Sort(this IQueryable<User> users, string? orderByQueryString)
	{
		if (string.IsNullOrWhiteSpace(orderByQueryString))
			return users.OrderBy(e => e.UserName); 

		var orderQuery = OrderQueryBuilder.CreateOrderQuery<User>(orderByQueryString);

		if (string.IsNullOrWhiteSpace(orderQuery))
			return users.OrderBy(e => e.UserName); 

		return users.OrderBy(orderQuery); 
	}
}
