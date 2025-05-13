using CustomForms.Domain.Common;

namespace CustomForms.Domain.Templates;

public class Topic : BaseEntity
{
	public string Name { get; private set; }

	public virtual ICollection<Template> Templates { get; private set; } = [];

	public Topic(Guid id, string name) : base(id)
	{
		if (string.IsNullOrWhiteSpace(name)) 
			throw new ArgumentException("Topic name cannot be empty.", nameof(name));
		
		Name = name;
	}
}
