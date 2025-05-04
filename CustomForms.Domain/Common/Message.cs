using Microsoft.AspNetCore.Http;

namespace CustomForms.Domain.Common;

public class Message(IEnumerable<string> to, string subject, string content, IFormFileCollection? attachments)
{
	public List<string> To { get; set; } = [.. to];
	public string Subject { get; set; } = subject;
	public string Content { get; set; } = content;
	public IFormFileCollection? Attachments { get; set; } = attachments;
}
