namespace CustomForms.Domain.Common.Exceptions;

public sealed class SendEmaiMessageException() : 
	Exception("Error with SmtpClient during sending email message")
{
}
