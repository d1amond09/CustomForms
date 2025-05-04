using System.Collections;
using System.Dynamic;

namespace CustomForms.Domain.Common;

public class Entity : DynamicObject, IDictionary<string, object>
{
	private readonly IDictionary<string, object> _expando;
	public ICollection<string> Keys => _expando.Keys;
	public ICollection<object> Values => _expando.Values;
	public int Count => _expando.Count;
	public bool IsReadOnly => _expando.IsReadOnly;

	public Entity()
	{
		_expando = new ExpandoObject()!;
	}

	public object this[string key]
	{
		get => _expando[key];
		set => _expando[key] = value;
	}

	public override bool TryGetMember(GetMemberBinder binder, out object? result)
	{
		if (_expando.TryGetValue(binder.Name, out object? value))
		{
			result = value;
			return true;
		}
		return base.TryGetMember(binder, out result);
	}

	public override bool TrySetMember(SetMemberBinder binder, object? value)
	{
		ArgumentNullException.ThrowIfNull(value);
		_expando[binder.Name] = value;
		return true;
	}

	public void Add(string key, object value) =>
		_expando.Add(key, value);
	public void Add(KeyValuePair<string, object> item) =>
		_expando.Add(item);

	public bool Remove(string key) =>
		_expando.Remove(key);
	public bool Remove(KeyValuePair<string, object> item) =>
		_expando.Remove(item);

	public bool Contains(KeyValuePair<string, object> item) =>
		_expando.Contains(item);
	public bool ContainsKey(string key) =>
		_expando.ContainsKey(key);

	public bool TryGetValue(string key, out object value) =>
		_expando.TryGetValue(key, out value!);

	public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex) =>
		_expando.CopyTo(array, arrayIndex);

	public void Clear() =>
		_expando.Clear();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	public IEnumerator<KeyValuePair<string, object>> GetEnumerator() =>
		_expando.GetEnumerator();
}