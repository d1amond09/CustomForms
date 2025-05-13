using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CustomForms.Application.Common.Responses;

public class DropboxUploadSuccessResponse
{
	[JsonPropertyName("name")]
	public string Name { get; set; } = string.Empty;
	[JsonPropertyName("path_lower")]
	public string PathLower { get; set; } = string.Empty;
	[JsonPropertyName("path_display")]
	public string PathDisplay { get; set; } = string.Empty;
	[JsonPropertyName("id")]
	public string Id { get; set; } = string.Empty;
}
