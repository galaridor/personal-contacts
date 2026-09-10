using Contacts.Application.Common.Models;
using Contacts.Application.Contacts.Models;

namespace Contacts.Application.Common.Abstractions
{
	public interface IContactQueries
	{
		Task<PagedResult<ContactDto>> ListAsync(
			string? search,
			int page,
			int pageSize,
			CancellationToken cancellationToken = default);

		Task<ContactDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
	}
}
