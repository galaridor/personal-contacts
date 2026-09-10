using Contacts.Application.Common.Abstractions;
using Contacts.Application.Common.Results;
using Contacts.Application.Contacts.Mapping;
using Contacts.Application.Contacts.Models;
using Contacts.Domain.Entities;
using Contacts.Domain.ValueObjects;
using FluentValidation;

namespace Contacts.Application.Contacts.Commands.CreateContact
{
	public sealed class CreateContactCommandHandler(
		IContactRepository repository,
		IUnitOfWork unitOfWork,
		IValidator<CreateContactCommand> validator,
		TimeProvider timeProvider)
	{
		public async Task<Result<ContactDto>> HandleAsync(
			CreateContactCommand command,
			CancellationToken cancellationToken = default)
		{
			CreateContactCommand normalized = command.Normalized();

			await validator.ValidateAndThrowAsync(normalized, cancellationToken);

			Contact contact = Contact.Create(
				normalized.FirstName,
				normalized.Surname,
				normalized.DateOfBirth,
				Address.Create(
					normalized.Address.Street,
					normalized.Address.HouseNumber,
					normalized.Address.PostalCode,
					normalized.Address.City,
					normalized.Address.Country),
				PhoneNumber.Create(normalized.PhoneNumber),
				Iban.Create(normalized.Iban),
				timeProvider.GetUtcNow());

			repository.Add(contact);

			await unitOfWork.SaveChangesAsync(cancellationToken);

			return contact.ToDto();
		}
	}
}
