using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CustomForms.Application.Common.Interfaces;
using CustomForms.Domain.Common.Constants;
using CustomForms.Domain.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace CustomForms.Infrastructure.Security.CurrentUserService;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor, UserManager<User> userManager) : ICurrentUserService
{
	private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
	private readonly UserManager<User> _userManager = userManager;

	private Guid? _cachedUserId = null;
	private bool _userIdCached = false;
	private User? _cachedUser = null;
	private bool _userCached = false;

	private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

	public Guid? UserId
	{
		get
		{
			if (_userIdCached) return _cachedUserId;

			_cachedUserId = GetUserIdFromClaims();
			_userIdCached = true;
			return _cachedUserId;
		}
	}

	public string? UserName => User?.Identity?.Name;

	public string? UserEmail => User?.FindFirstValue(ClaimTypes.Email);

	public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

	public IEnumerable<Claim>? GetUserClaims() => User?.Claims;

	public async Task<bool> IsInRoleAsync(string roleName)
	{
		if (User?.IsInRole(roleName) ?? false)
		{
			return true;
		}

		var user = await GetCurrentUserAsync();
		if (user == null)
		{
			return false;
		}
		return await _userManager.IsInRoleAsync(user, roleName);
	}

	public async Task<bool> IsAdminAsync()
	{
		return await IsInRoleAsync(Roles.Admin);
	}

	public async Task<User?> GetCurrentUserAsync(CancellationToken cancellationToken = default)
	{
		if (_userCached) return _cachedUser;

		var currentUserId = UserId;
		if (currentUserId == null)
		{
			_cachedUser = null;
			_userCached = true;
			return null;
		}

		_cachedUser = await _userManager.FindByIdAsync(currentUserId.Value.ToString());
		_userCached = true;
		return _cachedUser;
	}

	private Guid? GetUserIdFromClaims()
	{
		var userIdClaim = User?.FindFirstValue(ClaimTypes.NameIdentifier);
		if (Guid.TryParse(userIdClaim, out var userId))
		{
			return userId;
		}
		return null;
	}
}
