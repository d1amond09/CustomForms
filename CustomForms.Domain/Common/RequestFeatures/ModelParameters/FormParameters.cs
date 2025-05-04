namespace CustomForms.Domain.Common.RequestFeatures.ModelParameters;

public class FormParameters : RequestParameters
{
	public Guid? TemplateId { get; set; }
	public Guid? UserId { get; set; }
	public DateTime? MinFilledDate { get; set; }
	public DateTime? MaxFilledDate { get; set; }

	public FormParameters()
	{
		OrderBy = "FilledDate desc";	
	}
}
