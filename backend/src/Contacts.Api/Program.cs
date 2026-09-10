using Contacts.Api.Endpoints;
using Contacts.Api.Extensions;
using Contacts.Application;
using Contacts.Infrastructure;
using Contacts.Infrastructure.Persistence;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services
	.AddApiServices(builder.Configuration)
	.AddApplication()
	.AddInfrastructure();

WebApplication app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "Personal Contacts API"));
}

app.UseCors(ServiceCollectionExtensions.FrontendCorsPolicy);
app.MapContactsEndpoints();

// In production this runs as a deploy step
if (app.Configuration.GetValue("Database:AutoMigrate", defaultValue: true))
{
	await DatabaseInitializer.MigrateAsync(app.Services);
}

await app.RunAsync();
