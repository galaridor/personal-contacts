using Contacts.Application.Common.Abstractions;
using Contacts.Application.Common.Errors;
using Contacts.Application.Common.Results;
using Contacts.Application.Contacts.Mapping;
using Contacts.Application.Contacts.Models;
using Contacts.Domain.Entities;
using Contacts.Domain.ValueObjects;
using FluentValidation;

namespace Contacts.Application.Contacts.Commands.UpdateContact
{
	public sealed class UpdateContactCommandHandler(
		IContactRepository repository,
		IUnitOfWork unitOfWork,
		IValidator<UpdateContactCommand> validator,
		TimeProvider timeProvider)
	{
		public async Task<Result<ContactDto>> HandleAsync(
			UpdateContactCommand command,
			CancellationToken cancellationToken = default)
		{
			UpdateContactCommand normalized = command.Normalized();

			await validator.ValidateAndThrowAsync(normalized, cancellationToken);

			Contact? contact = await repository.GetByIdAsync(normalized.Id, cancellationToken);

			if (contact is null)
			{
				return Result.Failure<ContactDto>(ContactErrors.NotFound(normalized.Id));
			}

			contact.Update(
				normalized.FirstName,
				normalized.Surname,
				normalized.DateOfBirth,
				Address.Create(
					normalized.Address?.Street,
					normalized.Address?.HouseNumber,
					normalized.Address?.PostalCode,
					normalized.Address?.City,
					normalized.Address?.Country),
				PhoneNumber.Create(normalized.PhoneNumber),
				Iban.Create(normalized.Iban),
				timeProvider.GetUtcNow());

			await unitOfWork.SaveChangesAsync(cancellationToken);

			return contact.ToDto();
		}
	}
}
