using Contacts.Api.Middleware;
using Microsoft.OpenApi;

namespace Contacts.Api.Extensions
{
	internal static class ServiceCollectionExtensions
	{
		public const string FrontendCorsPolicy = "contacts-web";

		public static IServiceCollection AddApiServices(
			this IServiceCollection services,
			IConfiguration configuration)
		{
			// Every error response is a ProblemDetails
			services.AddProblemDetails();
			services.AddExceptionHandler<GlobalExceptionHandler>();

			// Swagger for testing purpsoes
			services.AddEndpointsApiExplorer();
			services.AddSwaggerGen(options => options.SwaggerDoc("v1", new OpenApiInfo
			{
				Title = "Personal Contacts API",
				Version = "v1",
				Description = "CRUD API for personal contacts.",
			}));

			string[] allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
				?? ["http://localhost:4200"];

			services.AddCors(options => options.AddPolicy(
				FrontendCorsPolicy,
				policy => policy
					.WithOrigins(allowedOrigins)
					.AllowAnyHeader()
					.AllowAnyMethod()));

			return services;
		}
	}
}
