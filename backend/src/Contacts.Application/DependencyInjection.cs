using Contacts.Application.Contacts.Commands.CreateContact;
using Contacts.Application.Contacts.Commands.DeleteContact;
using Contacts.Application.Contacts.Commands.UpdateContact;
using Contacts.Application.Contacts.Queries.GetContact;
using Contacts.Application.Contacts.Queries.GetContacts;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Contacts.Application
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddApplication(this IServiceCollection services)
		{
			services.AddValidatorsFromAssemblyContaining<CreateContactCommandValidator>(ServiceLifetime.Scoped);

			// Register handlers, which are injected into the API endpoints
			services.AddScoped<CreateContactCommandHandler>();
			services.AddScoped<UpdateContactCommandHandler>();
			services.AddScoped<DeleteContactCommandHandler>();
			services.AddScoped<GetContactQueryHandler>();
			services.AddScoped<GetContactsQueryHandler>();

			// Injected rather than read from DateTimeOffset.UtcNow
			services.TryAddTimeProvider();

			return services;
		}

		private static void TryAddTimeProvider(this IServiceCollection services)
		{
			if (services.All(x => x.ServiceType != typeof(TimeProvider)))
			{
				services.AddSingleton(TimeProvider.System);
			}
		}
	}
}
