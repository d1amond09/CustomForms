using CustomForms.Application.Common.DTOs;

namespace CustomForms.Application.Common.Interfaces;

public interface ICloudStorageService
{
	Task<CloudUploadResult> UploadJsonFileAsync(string fileName, string jsonContent, CancellationToken cancellationToken);

	IEnumerable<string> GetAdminEmails();
}
