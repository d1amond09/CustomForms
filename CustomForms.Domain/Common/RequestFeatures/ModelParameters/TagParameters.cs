namespace CustomForms.Domain.Common.RequestFeatures.ModelParameters;

public class TagParameters : RequestParameters
{
	public string? SearchTerm { get; set; }
	public bool? OnlyPopular { get; set; } 

	public TagParameters()
	{
		OrderBy = "Name";
	}
}
