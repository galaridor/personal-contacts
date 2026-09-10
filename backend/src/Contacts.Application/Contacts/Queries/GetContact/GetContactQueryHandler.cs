using Contacts.Application.Common.Abstractions;
using Contacts.Application.Common.Errors;
using Contacts.Application.Common.Results;
using Contacts.Application.Contacts.Models;

namespace Contacts.Application.Contacts.Queries.GetContact
{
	public sealed class GetContactQueryHandler(IContactQueries queries)
	{
		public async Task<Result<ContactDto>> HandleAsync(
			GetContactQuery query,
			CancellationToken cancellationToken = default)
		{
			ContactDto? contact = await queries.GetByIdAsync(query.Id, cancellationToken);

			return contact is null
				? Result.Failure<ContactDto>(ContactErrors.NotFound(query.Id))
				: Result.Success(contact);
		}
	}
}
