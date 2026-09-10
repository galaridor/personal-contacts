using Contacts.Application.Contacts.Validation;
using FluentValidation;

namespace Contacts.Application.Contacts.Commands.UpdateContact
{
	public sealed class UpdateContactCommandValidator : AbstractValidator<UpdateContactCommand>
	{
		public UpdateContactCommandValidator(TimeProvider timeProvider)
		{
			RuleFor(command => command.Id)
				.NotEmpty().WithMessage("Contact Id is required.");

			ContactRules.ApplyTo(this, timeProvider);
		}
	}
}
