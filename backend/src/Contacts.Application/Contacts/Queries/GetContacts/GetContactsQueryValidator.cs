using Contacts.Application.Common.Models;
using FluentValidation;

namespace Contacts.Application.Contacts.Queries.GetContacts
{
	public sealed class GetContactsQueryValidator : AbstractValidator<GetContactsQuery>
	{
		public GetContactsQueryValidator()
		{
			RuleFor(query => query.Page)
				.GreaterThanOrEqualTo(1).WithMessage("Page must be 1 or greater.");

			RuleFor(query => query.PageSize)
				.GreaterThanOrEqualTo(1).WithMessage("Page size must be 1 or greater.")
				.LessThanOrEqualTo(Paging.MaxPageSize)
				.WithMessage($"Page size cannot exceed {Paging.MaxPageSize}.");
		}
	}
}
