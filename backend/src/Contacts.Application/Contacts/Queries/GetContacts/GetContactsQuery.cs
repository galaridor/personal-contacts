using Contacts.Application.Common.Models;

namespace Contacts.Application.Contacts.Queries.GetContacts
{
	public sealed record GetContactsQuery(
		string? Search = null,
		int Page = Paging.DefaultPage,
		int PageSize = Paging.DefaultPageSize);
}
