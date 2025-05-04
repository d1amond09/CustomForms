using System.Text.Json.Serialization;
using CustomForms.API.GlobalException;
using Microsoft.OpenApi.Models;

namespace CustomForms.API;

public static class DependencyInjection
{
	public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddSwagger();
		services.AddCors(options =>
		{
			options.AddPolicy("CorsPolicy", builder =>
			builder.WithOrigins(configuration.GetSection("Frontend").GetValue<string>("Default")!)
			.AllowAnyMethod()
			.AllowAnyHeader()
			.WithExposedHeaders("X-Pagination"));
		});

		services.AddAuthentication();

		services.AddControllers()
			.AddJsonOptions(opts => 
			opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

		services.AddEndpointsApiExplorer();
		services.AddSwaggerGen();

		services.AddProblemDetails();
		services.AddExceptionHandler<GlobalExceptionHandler>();

		return services;
	}

	public static IServiceCollection AddSwagger(this IServiceCollection services)
	{
		services.AddSwaggerGen(s =>
		{
			s.SwaggerDoc("v1", new OpenApiInfo
			{
				Title = "Custom Forms API",
				Version = "v1"
			});
			s.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

			s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
			{
				In = ParameterLocation.Header,
				Description = "Place to add JWT with Bearer",
				Name = "Authorization",
				Type = SecuritySchemeType.ApiKey,
				Scheme = "Bearer"
			});
			s.AddSecurityRequirement(new OpenApiSecurityRequirement() { {
				new OpenApiSecurityScheme {
					Reference = new OpenApiReference {
						Type = ReferenceType.SecurityScheme,
						Id = "Bearer"
					},
					Name = "Bearer",
				},
				new List<string>()
			} });
		});

		return services;
	}

}
