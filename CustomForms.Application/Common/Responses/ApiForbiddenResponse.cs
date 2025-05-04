namespace CustomForms.Application.Common.Responses;

public class ApiForbiddenResponse(string message) : ApiBaseResponse(false)
{
    public string Message { get; set; } = message;
}
