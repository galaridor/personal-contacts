using Contacts.Application.Contacts.Validation;
using FluentValidation;

namespace Contacts.Application.Contacts.Commands.CreateContact
{
	public sealed class CreateContactCommandValidator : AbstractValidator<CreateContactCommand>
	{
		public CreateContactCommandValidator(TimeProvider timeProvider)
		{
			ContactRules.ApplyTo(this, timeProvider);
		}
	}
}
