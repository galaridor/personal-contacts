using Contacts.Application.Contacts.Models;
using Contacts.Application.Contacts.Validation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contacts.Application.Contacts.Commands.CreateContact
{
	public sealed record CreateContactCommand(
		string FirstName,
		string Surname,
		DateOnly DateOfBirth,
		AddressModel Address,
		string PhoneNumber,
		string Iban) : IContactCommand;
}
