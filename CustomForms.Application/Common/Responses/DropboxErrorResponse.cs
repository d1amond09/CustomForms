using System.Text.Json.Serialization;

namespace CustomForms.Application.Common.Responses;

public class DropboxErrorResponse
{
	[JsonPropertyName("error_summary")]
	public string ErrorSummary { get; set; } = string.Empty;
}
