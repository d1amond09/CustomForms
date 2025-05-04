using CustomForms.Domain.Forms;

namespace CustomForms.Domain.Common.Exceptions;

public class QuestionLimitExceededException(QuestionType type, int limit) : 
	BadRequestException($"Cannot add more questions of type '{type}'. The limit is {limit}.")
{
}
