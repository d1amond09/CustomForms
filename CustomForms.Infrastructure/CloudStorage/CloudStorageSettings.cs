using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomForms.Infrastructure.CloudStorage;

public class CloudStorageSettings
{
	public string Provider { get; set; } = "Dropbox";
	public DropboxSettings Dropbox { get; set; } = new();
	public string AdminEmails { get; set; } = string.Empty;
}

public class DropboxSettings
{
	public string AppKey { get; set; } = string.Empty;
	public string AppSecret { get; set; } = string.Empty;
	public string AccessToken { get; set; } = string.Empty;
	public string UploadFolderPath { get; set; } = "/SupportTickets";
}