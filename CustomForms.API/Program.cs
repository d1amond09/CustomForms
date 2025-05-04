using CustomForms.API;
using CustomForms.Application;
using CustomForms.Infrastructure;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);
{
	builder.Services
		.AddPresentation(builder.Configuration)
		.AddInfrastructure(builder.Configuration)
		.AddApplication()
		;
}


var app = builder.Build();
{
	app.UseExceptionHandler();
	app.UseCors("CorsPolicy");

	if (app.Environment.IsDevelopment())
	{
		app.UseSwagger();
		app.UseSwaggerUI();
	}

	app.UseHttpsRedirection();
	app.UseForwardedHeaders(new ForwardedHeadersOptions
	{
		ForwardedHeaders = ForwardedHeaders.All
	});
	app.UseRouting();
	app.UseAuthentication();
	app.UseAuthorization();
	app.MapControllers();

	app.Run();
}

