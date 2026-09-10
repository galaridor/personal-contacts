using Contacts.Application.Common.Abstractions;
using Contacts.Application.Common.Errors;
using Contacts.Application.Common.Results;
using Contacts.Domain.Entities;

namespace Contacts.Application.Contacts.Commands.DeleteContact
{
	public sealed class DeleteContactCommandHandler(IContactRepository repository, IUnitOfWork unitOfWork)
	{
		public async Task<Result> HandleAsync(DeleteContactCommand command, CancellationToken cancellationToken = default)
		{
			Contact? contact = await repository.GetByIdAsync(command.Id, cancellationToken);

			if (contact is null)
			{
				return Result.Failure(ContactErrors.NotFound(command.Id));
			}

			repository.Remove(contact);

			await unitOfWork.SaveChangesAsync(cancellationToken);

			return Result.Success();
		}
	}
}
