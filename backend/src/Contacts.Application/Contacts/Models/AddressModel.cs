namespace Contacts.Application.Contacts.Models
{
	public sealed record AddressModel(
		string Street,
		string HouseNumber,
		string PostalCode,
		string City,
		string Country);

}
