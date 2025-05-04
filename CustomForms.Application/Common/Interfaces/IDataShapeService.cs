using CustomForms.Domain.Common;

namespace CustomForms.Application.Common.Interfaces;

public interface IDataShapeService<T>
{
	IEnumerable<ShapedEntity> ShapeData(IEnumerable<T> entities, string fieldsString);
	ShapedEntity ShapeData(T entity, string fieldsString);
}
