namespace CustomForms.Domain.Common.Exceptions;

public class AuthorizationException(string message) : BadRequestException(message)
{
}
