using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomForms.Application.Common.DTOs;

public class CloudUploadResult
{
	public bool Success { get; set; }
	public string? FilePath { get; set; }
	public string? ErrorMessage { get; set; }
}
