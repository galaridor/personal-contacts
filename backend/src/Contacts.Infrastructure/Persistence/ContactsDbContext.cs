using Contacts.Application.Common.Abstractions;
using Contacts.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contacts.Infrastructure.Persistence
{
	internal sealed class ContactsDbContext(DbContextOptions<ContactsDbContext> options)
		: DbContext(options), IUnitOfWork
	{
		public DbSet<Contact> Contacts => Set<Contact>();

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.ApplyConfigurationsFromAssembly(typeof(ContactsDbContext).Assembly);

			base.OnModelCreating(modelBuilder);
		}
	}
}
