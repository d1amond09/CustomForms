namespace CustomForms.Application.Common.Interfaces;

public interface IRepository<T> where T : class
{
	Task<IEnumerable<T>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges);
	Task<T?> GetByIdAsync(Guid id, bool trackChanges);
	void Create(T evnt);
	void Update(T evnt);
	void Delete(T evnt);
	Task SaveAsync();
}
