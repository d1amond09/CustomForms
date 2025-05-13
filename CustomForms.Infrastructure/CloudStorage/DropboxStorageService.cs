using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CustomForms.Application.Common.DTOs;
using CustomForms.Application.Common.Interfaces;
using CustomForms.Application.Common.Responses;
using Microsoft.Extensions.Options;

namespace CustomForms.Infrastructure.CloudStorage;

public class DropboxStorageService : ICloudStorageService
{
	private readonly HttpClient _httpClient;
	private readonly DropboxSettings _settings;
	private readonly CloudStorageSettings _storageSettings;

	public DropboxStorageService(HttpClient httpClient, IOptions<CloudStorageSettings> storageSettings)
	{
		_httpClient = httpClient;
		_settings = storageSettings.Value.Dropbox; 
		_storageSettings = storageSettings.Value;
		_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _settings.AccessToken);

	}

	public IEnumerable<string> GetAdminEmails()
	{
		var adminEmails = _storageSettings.AdminEmails
			.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
			.ToList();

		return adminEmails;
	}

	public async Task<CloudUploadResult> UploadJsonFileAsync(string fileName, string jsonContent, CancellationToken cancellationToken)
	{
		if (!fileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
		{
			fileName += ".json";
		}

		string fullPath = (_settings.UploadFolderPath.TrimEnd('/') + "/" + fileName.TrimStart('/')).Replace("//", "/");

		Console.WriteLine($"--- Sending to Dropbox ---");
		Console.WriteLine($"Path: {fullPath}");
		Console.WriteLine($"Content Length: {jsonContent.Length}");

		var apiArgsObject = new
		{
			path = fullPath,
			mode = "add",
			autorename = true,
			mute = false
		};

		var apiArgs = JsonSerializer.Serialize(apiArgsObject);

		var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json"); // Изначально можно указать application/json
																							   // Затем принудительно меняем Content-Type
		stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

		Console.WriteLine($"Dropbox-API-Arg: {apiArgs}");

		var request = new HttpRequestMessage(HttpMethod.Post, "https://content.dropboxapi.com/2/files/upload")
		{
			Headers = {
				{ "Dropbox-API-Arg", apiArgs }
			},
			Content = stringContent
		};

		try
		{
			var response = await _httpClient.SendAsync(request, cancellationToken);
			if (response.IsSuccessStatusCode)
			{
				var result = await response.Content.ReadFromJsonAsync<DropboxUploadSuccessResponse>(cancellationToken: cancellationToken);
				return new CloudUploadResult { Success = true, FilePath = result?.PathDisplay };
			}
			else
			{
				var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
				Console.WriteLine($"Dropbox Upload Error: {response.StatusCode} - {errorContent}");

				var errorResponse = JsonSerializer.Deserialize<DropboxErrorResponse>(errorContent);
				return new CloudUploadResult { Success = false, ErrorMessage = errorResponse?.ErrorSummary ?? errorContent };
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Exception during Dropbox upload: {ex.Message}");
			return new CloudUploadResult { Success = false, ErrorMessage = ex.Message };
		}
	}
}
