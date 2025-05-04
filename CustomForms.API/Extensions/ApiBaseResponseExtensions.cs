

using CustomForms.Application.Common.Responses;

namespace CustomForms.API.Extensions;

public static class ApiBaseResponseExtensions
{
	public static TResultType GetResult<TResultType>(this ApiBaseResponse apiBaseResponse) =>
		((ApiOkResponse<TResultType>)apiBaseResponse).Result;

}
