using Contacts.Application.Contacts.Models;
using Contacts.Domain.Entities;
using FluentValidation;

namespace Contacts.Application.Contacts.Validation
{
	internal static class ContactRules
	{
		public static void ApplyTo<TCommand>(AbstractValidator<TCommand> validator, TimeProvider timeProvider)
			where TCommand : IContactCommand
		{
			validator.RuleFor(command => command.FirstName)
				.NotEmpty().WithMessage("First name is required.")
				.MaximumLength(Contact.MaxNameLength);

			validator.RuleFor(command => command.Surname)
				.NotEmpty().WithMessage("Surname is required.")
				.MaximumLength(Contact.MaxNameLength);

			validator.RuleFor(command => command.DateOfBirth)
				.NotEqual(default(DateOnly)).WithMessage("Date of birth is required.")
				.LessThanOrEqualTo(_ => DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime))
				.WithMessage("Date of birth cannot be in the future.");

			validator.RuleFor(command => command.Address)
				.NotNull().WithMessage("Address is required.")
				.SetValidator(new AddressModelValidator()!);

			validator.RuleFor(command => command.PhoneNumber)
				.NotEmpty().WithMessage("Phone number is required.");

			validator.RuleFor(command => command.Iban)
				.NotEmpty().WithMessage("IBAN is required.");
		}
	}

	internal interface IContactCommand
	{
		string? FirstName { get; }

		string? Surname { get; }

		DateOnly DateOfBirth { get; }

		AddressModel? Address { get; }

		string? PhoneNumber { get; }

		string? Iban { get; }
	}
}
