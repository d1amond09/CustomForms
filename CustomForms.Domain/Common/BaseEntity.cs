namespace CustomForms.Domain.Common;

public abstract class BaseEntity
{
	public Guid Id { get; protected set; }

	protected BaseEntity(Guid id)
	{
		Id = id == Guid.Empty ? Guid.NewGuid() : id;
	}

	// Parameterless constructor for ORM/Serialization
	protected BaseEntity() : this(Guid.NewGuid()) { }

	// Equality members based on Id
	public override bool Equals(object? obj)
	{
		if (obj is not BaseEntity other)
			return false;

		if (ReferenceEquals(this, other))
			return true;

		if (GetUnproxiedType(GetType()) != GetUnproxiedType(other.GetType()))
			return false;

		if (Id.Equals(default) || other.Id.Equals(default))
			return false;

		return Id.Equals(other.Id);
	}

	public override int GetHashCode() =>
		(GetUnproxiedType(GetType()).ToString() + Id).GetHashCode();

	public static bool operator ==(BaseEntity? a, BaseEntity? b)
	{
		if (a is null && b is null) return true;
		if (a is null || b is null) return false;
		return a.Equals(b);
	}

	public static bool operator !=(BaseEntity? a, BaseEntity? b) => !(a == b);

	// Helper for ORM proxies (like EF Core lazy loading proxies)
	internal static Type GetUnproxiedType(Type type)
	{
		// Specific check for EF Core proxies, adjust if using another ORM
		// or remove if lazy loading proxies aren't used.
		if (type.Namespace == "Castle.Proxies" || type.Name.EndsWith("Proxy"))
		{
			return type.BaseType ?? type; // Return Base type if it's a proxy
		}
		return type;
	}
}
