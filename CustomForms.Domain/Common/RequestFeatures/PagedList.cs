using Microsoft.EntityFrameworkCore;

namespace CustomForms.Domain.Common.RequestFeatures;

public class PagedList<T> : List<T>
{
	public MetaData MetaData { get; set; }
	public PagedList(List<T> items, int count, int pageNumber, int pageSize)
	{
		MetaData = new MetaData
		{
			TotalCount = count,
			PageSize = pageSize,
			CurrentPage = pageNumber,
		};
		AddRange(items);
	}
	public static PagedList<T> ToPagedList(IEnumerable<T> source, int pageNumber, int pageSize)
	{
		var count = source.Count();
		var items = source
			.Skip((pageNumber - 1) * pageSize)
			.Take(pageSize).ToList();
		return new PagedList<T>(items, count, pageNumber, pageSize);
	}

	public static async Task<PagedList<T>> ToPagedListAsync(IQueryable<T> source, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(source);

		var count = await source.CountAsync(cancellationToken);
		var items = await source
			.Skip((pageNumber - 1) * pageSize)
			.Take(pageSize)
			.ToListAsync(cancellationToken);

		return new PagedList<T>(items, count, pageNumber, pageSize);
	}
}

