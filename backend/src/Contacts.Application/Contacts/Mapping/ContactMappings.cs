using Contacts.Application.Contacts.Models;
using Contacts.Domain.Entities;

namespace Contacts.Application.Contacts.Mapping
{
	public static class ContactMappings
	{
		public static ContactDto ToDto(this Contact contact) => new(
			contact.Id,
			contact.FirstName,
			contact.Surname,
			contact.DateOfBirth,
			new AddressModel(
				contact.Address.Street,
				contact.Address.HouseNumber,
				contact.Address.PostalCode,
				contact.Address.City,
				contact.Address.Country),
			contact.PhoneNumber.Value,
			contact.Iban.Value,
			contact.CreatedAtUtc,
			contact.UpdatedAtUtc);
	}
}
