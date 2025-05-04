namespace CustomForms.Domain.Common.RequestFeatures.ModelParameters;

public class TopicParameters : RequestParameters
{
	public string? SearchTerm { get; set; }

	public TopicParameters()
	{
		OrderBy = "Name";
	}
}
