using CustomForms.Domain.Forms;
using Microsoft.AspNetCore.Identity;

namespace CustomForms.Domain.Users;

public class User : IdentityUser<Guid> 
{
	public string FirstName { get; set; } = string.Empty;
	public string LastName { get; set; } = string.Empty;

	public string DisplayName => 
		string.IsNullOrWhiteSpace($"{FirstName} {LastName}") ? UserName : $"{FirstName} {LastName}".Trim();

	public string? RefreshToken { get; set; }
	public DateTime RefreshTokenExpiryTime { get; set; }

	public bool IsBlocked { get; private set; }

	public virtual ICollection<Template> CreatedTemplates { get; private set; } = [];
	public virtual ICollection<Form> FilledForms { get; private set; } = [];
	public virtual ICollection<Comment> Comments { get; private set; } = [];
	public virtual ICollection<Like> Likes { get; private set; } = [];

	public virtual ICollection<Template> AllowedTemplates { get; private set; } = [];

	public User() : base()
	{
		InitializeCollections();
	}

	public User(string userName, string email) : base(userName)
	{
		Email = email;
		NormalizedUserName = userName?.ToUpperInvariant();
		NormalizedEmail = email?.ToUpperInvariant();
		InitializeCollections();
	}

	private void InitializeCollections()
	{
		CreatedTemplates ??= [];
		FilledForms ??= [];
		Comments ??= [];
		Likes ??= [];
		AllowedTemplates ??= [];
	}

	public void Block()
	{
		IsBlocked = true;
	}

	public void Unblock()
	{
		IsBlocked = false;
	}

	public override bool Equals(object? obj)
	{
		if (obj is not User other)
			return false;

		if (ReferenceEquals(this, other))
			return true;

		var thisIsTransient = Id.Equals(default);
		var otherIsTransient = other.Id.Equals(default);

		if (thisIsTransient && otherIsTransient)
			return ReferenceEquals(this, other); 

		if (thisIsTransient || otherIsTransient || !Id.Equals(other.Id))
			return false;

		var otherType = GetUnproxiedType(other.GetType());
		var thisType = GetUnproxiedType(GetType());
		return thisType.IsAssignableFrom(otherType) || otherType.IsAssignableFrom(thisType);
	}

	public override int GetHashCode()
	{
		if (!Id.Equals(default))
		{
			return Id.GetHashCode() ^ 31; 
		}
		return base.GetHashCode();
	}

	internal static Type GetUnproxiedType(Type type)
	{
		if (type.Namespace == "Castle.Proxies" || type.Name.EndsWith("Proxy") || type.FullName?.Contains("DynamicProxies") == true)
		{
			return type.BaseType ?? type;
		}
		return type;
	}

	public static bool operator ==(User? a, User? b)
	{
		if (a is null && b is null) return true;
		if (a is null || b is null) return false;
		return a.Equals(b);
	}

	public static bool operator !=(User? a, User? b) => !(a == b);

	public bool IsAdmin() => false;

}