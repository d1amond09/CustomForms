using CustomForms.Domain.Common;

namespace CustomForms.Domain.Forms;

public class Tag : AuditableEntity 
{
	public string Name { get; private set; }

	public virtual ICollection<Template> Templates { get; private set; } = new List<Template>();

	public Tag(Guid id, string name) : base(id)
	{
		if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Tag name cannot be empty.", nameof(name));
		Name = name.Trim();
	}

	protected Tag() { }

	public void UpdateName(string name)
	{
		if (!string.IsNullOrWhiteSpace(name))
		{
			Name = name.Trim();
			SetModifiedDate();
		}
	}
}
