using Contacts.Application.Common.Abstractions;
using Contacts.Domain.Entities;
using Contacts.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Contacts.Infrastructure.Repositories
{
	internal sealed class ContactRepository(ContactsDbContext dbContext) : IContactRepository
	{
		public Task<Contact?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
			dbContext.Contacts.FirstOrDefaultAsync(contact => contact.Id == id, cancellationToken);

		public void Add(Contact contact) => dbContext.Contacts.Add(contact);

		public void Remove(Contact contact) => dbContext.Contacts.Remove(contact);
	}
}
