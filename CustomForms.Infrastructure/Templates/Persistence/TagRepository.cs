using CustomForms.Application.Common.Interfaces;
using CustomForms.Domain.Common.RequestFeatures.ModelParameters;
using CustomForms.Domain.Common.RequestFeatures;
using CustomForms.Domain.Forms;
using CustomForms.Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;
using CustomForms.Infrastructure.Common.Persistence.Extensions;

namespace CustomForms.Infrastructure.Templates.Persistence;

public class TagRepository(AppDbContext dbContext) : RepositoryBase<Tag>(dbContext), ITagRepository
{
	public async Task<PagedList<Tag>> GetTagsAsync(TagParameters tagParameters, CancellationToken cancellationToken = default)
	{
		var tagsQuery = FindAll(trackChanges: false)
		   .FilterTags(tagParameters) 
		   .Search(tagParameters.SearchTerm) 
		   .Sort(tagParameters.OrderBy, tagParameters.OnlyPopular); 

		return await PagedList<Tag>.ToPagedListAsync(tagsQuery,
		   tagParameters.PageNumber,
		   tagParameters.PageSize,
		   cancellationToken);
	}
	public async Task<Tag?> FindByNameAsync(string name, bool trackChanges = false, CancellationToken cancellationToken = default)
	{
		var normalizedName = name.Trim().ToLowerInvariant();
		return await FindByCondition(t => t.Name.ToLower() == normalizedName, trackChanges)
						.FirstOrDefaultAsync(cancellationToken);
	}

	public async Task<List<Tag>> SearchByNameAsync(string namePrefix, int maxResults, CancellationToken cancellationToken = default)
	{
		var normalizedPrefix = namePrefix.Trim().ToLowerInvariant();
		return await FindByCondition(t => t.Name.ToLower().StartsWith(normalizedPrefix), trackChanges: false)
			.OrderBy(t => t.Name)
			.Take(maxResults)
			.ToListAsync(cancellationToken);
	}

	public async Task<List<Tag>> FindOrCreateTagsAsync(IEnumerable<string> tagNames, CancellationToken cancellationToken = default)
	{
		var resultTags = new List<Tag>();
		var uniqueTrimmedNames = tagNames.Select(n => n.Trim()).Where(n => !string.IsNullOrEmpty(n)).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
		if (!uniqueTrimmedNames.Any()) return resultTags;
		var lowerCaseNamesToFind = uniqueTrimmedNames.Select(n => n.ToLowerInvariant()).ToList();
		var existingTags = await FindByCondition(t => lowerCaseNamesToFind.Contains(t.Name.ToLower()), trackChanges: true).ToListAsync(cancellationToken);
		resultTags.AddRange(existingTags);
		var existingLowerNames = existingTags.Select(t => t.Name.ToLowerInvariant()).ToHashSet();
		var namesToCreate = uniqueTrimmedNames.Where(n => !existingLowerNames.Contains(n.ToLowerInvariant())).ToList();
		foreach (var name in namesToCreate)
		{
			var originalName = tagNames.First(n => n.Trim().Equals(name, StringComparison.OrdinalIgnoreCase));
			var newTag = new Tag(Guid.NewGuid(), originalName);
			await CreateAsync(newTag, cancellationToken);
			resultTags.Add(newTag);
		}
		return resultTags;
	}

	public async Task<List<Tag>> GetPopularTagsAsync(int count, CancellationToken cancellationToken = default)
	{
		return await FindAll(trackChanges: false)
			.OrderByDescending(t => t.Templates.Count(tmpl => tmpl.IsPublic)) 
			.Take(count)
			.ToListAsync(cancellationToken);
	}
}
