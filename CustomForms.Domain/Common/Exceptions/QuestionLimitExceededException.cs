using CustomForms.Domain.Forms;
using CustomForms.Domain.Templates;

namespace CustomForms.Domain.Common.Exceptions;

public class QuestionLimitExceededException(QuestionType type, int limit) : 
	BadRequestException($"Cannot add more questions of type '{type}'. The limit is {limit}.")
{
}
