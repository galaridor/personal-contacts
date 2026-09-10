namespace Contacts.Api.Contracts.Contacts
{
	internal sealed record CreateContactRequest(
		string FirstName,
		string Surname,
		DateOnly DateOfBirth,
		AddressContract Address,
		string PhoneNumber,
		string Iban);
}
