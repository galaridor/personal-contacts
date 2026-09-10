namespace Contacts.Api.Contracts.Contacts
{
	internal sealed record AddressContract(
		string Street,
		string HouseNumber,
		string PostalCode,
		string City,
		string Country);
}
