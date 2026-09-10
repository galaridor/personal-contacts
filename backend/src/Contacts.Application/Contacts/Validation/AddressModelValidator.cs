using Contacts.Application.Contacts.Models;
using Contacts.Domain.ValueObjects;
using FluentValidation;

namespace Contacts.Application.Contacts.Validation
{
	public sealed class AddressModelValidator : AbstractValidator<AddressModel>
	{
		public AddressModelValidator()
		{
			RuleFor(address => address.Street)
				.NotEmpty().WithMessage("Street is required.")
				.MaximumLength(Address.MaxStreetLength);

			RuleFor(address => address.HouseNumber)
				.NotEmpty().WithMessage("House number is required.")
				.MaximumLength(Address.MaxHouseNumberLength);

			RuleFor(address => address.PostalCode)
				.NotEmpty().WithMessage("Postal code is required.")
				.MaximumLength(Address.MaxPostalCodeLength);

			RuleFor(address => address.City)
				.NotEmpty().WithMessage("City is required.")
				.MaximumLength(Address.MaxCityLength);

			RuleFor(address => address.Country)
				.NotEmpty().WithMessage("Country is required.")
				.Matches("^[A-Za-z]{2}$")
				.WithMessage("Country must be a two-letter country code, for example 'BG'.");
		}
	}
}
