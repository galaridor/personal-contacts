using Contacts.Application.Common.Abstractions;
using Contacts.Application.Common.Models;
using Contacts.Application.Contacts.Mapping;
using Contacts.Application.Contacts.Models;
using Contacts.Domain.Entities;
using Contacts.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Contacts.Infrastructure.Queries
{
	internal sealed class ContactQueries(ContactsDbContext dbContext) : IContactQueries
	{
		private const string LikeEscape = "\\";

		public async Task<PagedResult<ContactDto>> ListAsync(
			string? search,
			int page,
			int pageSize,
			CancellationToken cancellationToken = default)
		{
			IQueryable<Contact> query = dbContext.Contacts.AsNoTracking();

			if (!string.IsNullOrWhiteSpace(search))
			{
				//  At scale this has to change if the mathcing rules grow beyond "contains"
				string pattern = $"%{EscapeLikePattern(search.Trim())}%";
				query = query.Where(contact =>
					EF.Functions.Like(contact.FirstName.ToLower(), pattern, LikeEscape) ||
					EF.Functions.Like(contact.Surname.ToLower(), pattern, LikeEscape));
			}

			// The count runs against the same filter but without paging so the client can tell how many pages exist
			int totalCount = await query.CountAsync(cancellationToken);

			List<ContactDto> contacts = await query
				.OrderBy(contact => contact.Surname)
				.ThenBy(contact => contact.FirstName)
				.ThenBy(contact => contact.Id)
				.Skip((page - 1) * pageSize) // Other approach was to pass "start" and "lenght" instead of page and pageSize but this is more user-friendly
				.Take(pageSize)
				.Select(contact => contact.ToDto())
				.ToListAsync(cancellationToken);

			return new PagedResult<ContactDto>(
				[.. contacts],
				page,
				pageSize,
				totalCount);
		}

		public async Task<ContactDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
		{
			Contact? contact = await dbContext.Contacts
				.AsNoTracking()
				.FirstOrDefaultAsync(candidate => candidate.Id == id, cancellationToken);

			return contact?.ToDto();
		}

		private static string EscapeLikePattern(string value) => value
			.Replace(LikeEscape, LikeEscape + LikeEscape, StringComparison.Ordinal)
			.Replace("%", LikeEscape + "%", StringComparison.Ordinal)
			.Replace("_", LikeEscape + "_", StringComparison.Ordinal);
	}
}
