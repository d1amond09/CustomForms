namespace CustomForms.Domain.Common.Exceptions;

public abstract class BadRequestException(string message) : Exception(message)
{

}
