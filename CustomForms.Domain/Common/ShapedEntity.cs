namespace CustomForms.Domain.Common;

public class ShapedEntity
{
	public ShapedEntity()
	{
		Entity = [];
	}
	public Guid Id { get; set; }
	public Entity Entity { get; set; }
}
