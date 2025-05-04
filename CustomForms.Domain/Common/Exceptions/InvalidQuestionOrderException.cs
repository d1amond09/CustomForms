namespace CustomForms.Domain.Common.Exceptions;

public class InvalidQuestionOrderException(string message) : BadRequestException(message)
{
}
