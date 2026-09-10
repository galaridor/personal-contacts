using Contacts.Application.Contacts.Models;
using Contacts.Application.Contacts.Validation;

namespace Contacts.Application.Contacts.Commands.UpdateContact
{
	public sealed record UpdateContactCommand(
		Guid Id,
		string FirstName,
		string Surname,
		DateOnly DateOfBirth,
		AddressModel Address,
		string PhoneNumber,
		string Iban) : IContactCommand;

}
