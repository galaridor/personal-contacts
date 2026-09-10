using Contacts.Api.Contracts.Common;
using Contacts.Application.Common.Models;
using Contacts.Application.Contacts.Commands.CreateContact;
using Contacts.Application.Contacts.Commands.UpdateContact;
using Contacts.Application.Contacts.Models;

namespace Contacts.Api.Contracts.Contacts
{
	internal static class ContactContractMappings
	{
		public static CreateContactCommand ToCommand(this CreateContactRequest request) => new(
			request.FirstName,
			request.Surname,
			request.DateOfBirth,
			request.Address.ToModel(),
			request.PhoneNumber,
			request.Iban);

		public static UpdateContactCommand ToCommand(this UpdateContactRequest request, Guid id) => new(
			id,
			request.FirstName,
			request.Surname,
			request.DateOfBirth,
			request.Address.ToModel(),
			request.PhoneNumber,
			request.Iban);

		public static ContactResponse ToResponse(this ContactDto contact) => new(
			contact.Id,
			contact.FirstName,
			contact.Surname,
			contact.DateOfBirth,
			new AddressContract(
				contact.Address.Street,
				contact.Address.HouseNumber,
				contact.Address.PostalCode,
				contact.Address.City,
				contact.Address.Country),
			contact.PhoneNumber,
			contact.Iban,
			contact.CreatedAtUtc,
			contact.UpdatedAtUtc);

		public static PagedResponse<ContactResponse> ToResponse(this PagedResult<ContactDto> page) => new(
			[.. page.Items.Select(contact => contact.ToResponse())],
			page.Page,
			page.PageSize,
			page.TotalCount);

		private static AddressModel ToModel(this AddressContract address) => new(
				address.Street,
				address.HouseNumber,
				address.PostalCode,
				address.City,
				address.Country);
	}
}
