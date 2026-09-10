using Contacts.Application.Common.Results;

namespace Contacts.Application.Common.Errors
{
	public static class ContactErrors
	{
		public static Error NotFound(Guid id) => Error.NotFound(
			"contact.not_found",
			$"No contact was found with id '{id}'.");
	}

}
