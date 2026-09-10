namespace Contacts.Application.Contacts.Models
{
	public sealed record ContactDto(
		Guid Id,
		string FirstName,
		string Surname,
		DateOnly DateOfBirth,
		AddressModel Address,
		string PhoneNumber,
		string Iban,
		DateTimeOffset CreatedAtUtc,
		DateTimeOffset UpdatedAtUtc);
}
