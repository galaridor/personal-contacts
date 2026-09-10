using Contacts.Application.Common.Abstractions;
using Contacts.Infrastructure.Persistence;
using Contacts.Infrastructure.Queries;
using Contacts.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Contacts.Infrastructure
{
	public static class DependencyInjection
	{
		public const string ConnectionStringName = "ContactsDatabase";

		public static IServiceCollection AddInfrastructure(this IServiceCollection services)
		{
			services.AddDbContext<ContactsDbContext>((provider, options) =>
			{
				IConfiguration configuration = provider.GetRequiredService<IConfiguration>();
				string? connectionString = configuration.GetConnectionString(ConnectionStringName);

				if (string.IsNullOrWhiteSpace(connectionString))
				{
					throw new InvalidOperationException($"Connection string '{ConnectionStringName}' is not configured.");
				}

				options.UseNpgsql(connectionString);
			});

			services.AddScoped<IContactRepository, ContactRepository>();
			services.AddScoped<IContactQueries, ContactQueries>();
			services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ContactsDbContext>());

			return services;
		}
	}
}
