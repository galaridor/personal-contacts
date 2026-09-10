using Contacts.Application.Contacts.Commands.CreateContact;
using Contacts.Application.Contacts.Commands.UpdateContact;
using Contacts.Application.Contacts.Models;

namespace Contacts.Application.Contacts.Mapping
{
	public static class ContactCommandNormalization
	{
		public static CreateContactCommand Normalized(this CreateContactCommand command) => command with
		{
			FirstName = Clean(command.FirstName),
			Surname = Clean(command.Surname),
			Address = Normalized(command.Address),
			PhoneNumber = Clean(command.PhoneNumber),
			Iban = Clean(command.Iban),
		};

		public static UpdateContactCommand Normalized(this UpdateContactCommand command) => command with
		{
			FirstName = Clean(command.FirstName),
			Surname = Clean(command.Surname),
			Address = Normalized(command.Address),
			PhoneNumber = Clean(command.PhoneNumber),
			Iban = Clean(command.Iban),
		};

		private static AddressModel Normalized(AddressModel address) => address with
		{
			Street = Clean(address.Street),
			HouseNumber = Clean(address.HouseNumber),
			PostalCode = Clean(address.PostalCode),
			City = Clean(address.City),
			Country = Clean(address.Country).ToUpperInvariant(),
		};

		private static string Clean(string value) => value.Trim();
	}
}
