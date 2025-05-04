namespace CustomForms.Domain.Common.RequestFeatures.ModelParameters;

public class UserParameters : RequestParameters
{
	public string? SearchTerm { get; set; } 
	public bool? IsBlocked { get; set; }
	public string? Role { get; set; } 

	public UserParameters()
	{
		OrderBy = "UserName"; 
	}
}