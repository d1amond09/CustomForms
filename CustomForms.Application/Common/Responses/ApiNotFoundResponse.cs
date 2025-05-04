namespace CustomForms.Application.Common.Responses;

public class ApiNotFoundResponse(string message) : ApiBaseResponse(false)
{
    public string Message { get; set; } = message;
}
