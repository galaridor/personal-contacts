namespace Contacts.Api.Contracts.Contacts
{
	internal sealed record ContactResponse(
		Guid Id,
		string FirstName,
		string Surname,
		DateOnly DateOfBirth,
		AddressContract Address,
		string PhoneNumber,
		string Iban,
		DateTimeOffset CreatedAtUtc,
		DateTimeOffset UpdatedAtUtc);
}
