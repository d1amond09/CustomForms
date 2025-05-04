using Microsoft.EntityFrameworkCore;
using CustomForms.Domain.Users;
using CustomForms.Application.Common.Interfaces;
using CustomForms.Domain.Common.RequestFeatures.ModelParameters;
using CustomForms.Domain.Common.RequestFeatures;
using CustomForms.Infrastructure.Common.Persistence;
using Microsoft.AspNetCore.Identity;
using CustomForms.Infrastructure.Common.Persistence.Extensions;

namespace CustomForms.Infrastructure.Users.Persistence;

public class UserRepository(UserManager<User> userManager, AppDbContext dbContext) : IUserRepository
{
	private readonly UserManager<User> _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
	private readonly AppDbContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

	public IQueryable<User> GetUsersAsQueryable()
	{
		return _userManager.Users; 
	}

	public async Task<PagedList<User>> GetUsersAsync(UserParameters userParameters, CancellationToken cancellationToken = default)
	{
		var usersQuery = _userManager.Users
			.AsNoTracking() 
			.FilterUsers(userParameters) 
			.Search(userParameters.SearchTerm) 
			.Sort(userParameters.OrderBy); 

		return await PagedList<User>.ToPagedListAsync(usersQuery,
			userParameters.PageNumber,
			userParameters.PageSize,
			cancellationToken);
	}
	
	public async Task<User?> GetByIdWithNavigationsAsync(Guid id, CancellationToken cancellationToken = default)
	{
		return await _userManager.Users
			.Include(u => u.CreatedTemplates)
			.AsNoTracking()
			.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
	}

	public async Task<List<User>> SearchUsersForAutocompleteAsync(string searchTerm, int maxResults, CancellationToken cancellationToken = default)
	{
		var query = _userManager.Users.AsNoTracking();

		if (!string.IsNullOrWhiteSpace(searchTerm))
		{
			query = query.Search(searchTerm);
		}

		return await query
			.OrderBy(u => u.LastName).ThenBy(u => u.FirstName)
			.Take(maxResults)
			.ToListAsync(cancellationToken);
	}

	public async Task<User?> FindByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
	{
		return await _userManager.Users
			.AsNoTracking()
			.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken, cancellationToken);
	}

	public void Update(User user)
	{
		_dbContext.Users.Update(user);
	}
}
