using Contacts.Application.Common.Abstractions;
using Contacts.Application.Common.Models;
using Contacts.Application.Common.Results;
using Contacts.Application.Contacts.Models;
using FluentValidation;

namespace Contacts.Application.Contacts.Queries.GetContacts
{
	public sealed class GetContactsQueryHandler(
		IContactQueries queries,
		IValidator<GetContactsQuery> validator)
	{
		public async Task<Result<PagedResult<ContactDto>>> HandleAsync(
			GetContactsQuery query,
			CancellationToken cancellationToken = default)
		{
			await validator.ValidateAndThrowAsync(query, cancellationToken);

			PagedResult<ContactDto> page = await queries.ListAsync(
				query.Search,
				query.Page,
				query.PageSize,
				cancellationToken);

			return Result.Success(page);
		}
	}
}
