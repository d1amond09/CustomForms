using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomForms.Application.Common.DTOs;

public class CreateSupportTicketDto
{
	public string Summary { get; set; } = string.Empty;
	public string Priority { get; set; } = "Average"; 
	public string PageUrl { get; set; } = string.Empty; 
	public string? TemplateTitle { get; set; }
}

public class SupportTicketJsonDto
{
	public string ReportedBy { get; set; } = string.Empty;
	public string UserEmail { get; set; } = string.Empty;
	public string? Template { get; set; }
	public string Link { get; set; } = string.Empty;
	public string Priority { get; set; } = string.Empty;
	public DateTime ReportedAtUtc { get; set; }
	public List<string> NotifyAdmins { get; set; } = new List<string>();
	public string Summary { get; set; } = string.Empty;
}