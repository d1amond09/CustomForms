namespace CustomForms.Infrastructure.Security.TokenGenerator;

public class JwtConfiguration
{
	public const string Section = "JwtSettings";
	public string? ValidIssuer { get; set; }
	public string? ValidAudience { get; set; }
	public string? Expires { get; set; }
}

