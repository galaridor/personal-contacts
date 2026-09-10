using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Contacts.Infrastructure.Persistence
{
	public static class DatabaseInitializer
	{
		public static async Task MigrateAsync(IServiceProvider services, CancellationToken cancellationToken = default)
		{
			await using AsyncServiceScope scope = services.CreateAsyncScope();

			ILogger logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>()
				.CreateLogger(typeof(DatabaseInitializer));
			ContactsDbContext dbContext = scope.ServiceProvider.GetRequiredService<ContactsDbContext>();

			logger.LogInformation("Applying database migrations.");
			await dbContext.Database.MigrateAsync(cancellationToken);
			logger.LogInformation("Database is up to date.");
		}
	}
}
