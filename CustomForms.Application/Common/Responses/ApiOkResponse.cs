﻿namespace CustomForms.Application.Common.Responses;

public sealed class ApiOkResponse<TResult>(TResult result) : ApiBaseResponse(true)
{
    public TResult Result { get; set; } = result;
}

