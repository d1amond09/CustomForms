using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using CustomForms.Application.Common.Interfaces;
using dotenv.net;
using Microsoft.AspNetCore.Http;

namespace CustomForms.Infrastructure.Services;

public class CloudinaryService : ICloudinaryService
{
	private readonly Cloudinary _cloudinary;
	public CloudinaryService()
	{
		DotEnv.Load(options: new DotEnvOptions(probeForEnv: true));
		_cloudinary = new Cloudinary(Environment.GetEnvironmentVariable("CLOUDINARY_URL"));
		_cloudinary.Api.Secure = true;
	}

	public async Task<(string SecureUrl, string PublicId)?> UploadImageAsync(IFormFile file)
	{
		if (file == null || file.Length == 0) return null;

		await using var stream = file.OpenReadStream();
		var uploadParams = new ImageUploadParams
		{
			File = new FileDescription(file.FileName, stream),
			Folder = "templates",
		};

		var uploadResult = await _cloudinary.UploadAsync(uploadParams);

		if (uploadResult.Error != null)
		{
			return null;
		}

		return (uploadResult.SecureUrl.ToString(), uploadResult.PublicId);
	}

	public async Task<bool> DeleteImageAsync(string publicId)
	{
		if (string.IsNullOrEmpty(publicId)) return false;

		var deletionParams = new DeletionParams(publicId);
		try
		{
			var result = await _cloudinary.DestroyAsync(deletionParams);
			if (result.Result == "ok" || result.Result == "not found")
			{
				return true;
			}
			return false;
		}
		catch
		{
			return false;
		}
	}
}
