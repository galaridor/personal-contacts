using Contacts.Domain.Entities;

namespace Contacts.Application.Common.Abstractions
{
	public interface IContactRepository
	{
		Task<Contact?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

		void Add(Contact contact);

		void Remove(Contact contact);
	}
}
