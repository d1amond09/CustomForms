namespace CustomForms.Domain.Common.RequestFeatures.ModelParameters;

public class TemplateParameters : RequestParameters
{
	public string? SearchTerm { get; set; }
	public Guid? AuthorId { get; set; }
	public Guid? TopicId { get; set; }
	public Guid? TagId { get; set; }
	public bool? IsPublic { get; set; } = true; 
												

	public TemplateParameters()
	{
		OrderBy = "CreatedDate desc"; 
	}
}
