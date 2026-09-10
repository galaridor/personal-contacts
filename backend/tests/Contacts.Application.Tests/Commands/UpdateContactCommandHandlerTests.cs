using Contacts.Application.Common.Abstractions;
using Contacts.Application.Common.Results;
using Contacts.Application.Contacts.Commands.UpdateContact;
using Contacts.Application.Contacts.Models;
using Contacts.Domain.Entities;
using Contacts.Domain.Exceptions;
using FluentValidation;
using Moq;

namespace Contacts.Application.Tests.Commands
{
	public class UpdateContactCommandHandlerTests
	{
		private static readonly DateTimeOffset Later = TestData.NowUtc.AddDays(3);

		private readonly Mock<IContactRepository> _repository = new Mock<IContactRepository>();
		private readonly Mock<IUnitOfWork> _unitOfWork = new Mock<IUnitOfWork>();
		private readonly UpdateContactCommandHandler _handler;

		public UpdateContactCommandHandlerTests()
		{
			FixedTimeProvider clock = new FixedTimeProvider(Later);
			_handler = new UpdateContactCommandHandler(
				_repository.Object,
				_unitOfWork.Object,
				new UpdateContactCommandValidator(clock),
				clock);
		}

		[Fact]
		public async Task Applies_the_change_through_the_aggregate_and_commits()
		{
			Contact contact = Existing();

			Result<ContactDto> result = await _handler.HandleAsync(ValidCommand(contact.Id) with
			{
				Surname = "Byron",
				PhoneNumber = "+31 6 1111 2222",
			});

			Assert.True(result.IsSuccess);
			Assert.Equal("Byron", result.Value.Surname);
			Assert.Equal("+31611112222", result.Value.PhoneNumber);
			Assert.Equal(Later, result.Value.UpdatedAtUtc);
			Assert.Equal(TestData.NowUtc, result.Value.CreatedAtUtc);

			Assert.Equal("Byron", contact.Surname);
			_unitOfWork.Verify(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
		}

		[Fact]
		public async Task Reports_an_unknown_contact_as_a_failed_result_rather_than_throwing()
		{
			Guid id = Guid.NewGuid();
			_repository
				.Setup(repository => repository.GetByIdAsync(id, It.IsAny<CancellationToken>()))
				.ReturnsAsync((Contact?)null);

			Result<ContactDto> result = await _handler.HandleAsync(ValidCommand(id));

			Assert.True(result.IsFailure);
			Assert.Equal("contact.not_found", result.Error.Code);
			_unitOfWork.Verify(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
		}

		[Fact]
		public async Task Rejects_a_malformed_request_without_loading_the_contact()
		{
			await Assert.ThrowsAsync<ValidationException>(
				() => _handler.HandleAsync(ValidCommand(Guid.NewGuid()) with { Surname = "  " }));

			_repository.Verify(
				repository => repository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
				Times.Never);
		}


		private Contact Existing()
		{
			Contact contact = TestData.Contact();
			_repository
				.Setup(repository => repository.GetByIdAsync(contact.Id, It.IsAny<CancellationToken>()))
				.ReturnsAsync(contact);
			return contact;
		}

		private static UpdateContactCommand ValidCommand(Guid id) => new(
			id,
			"Milen",
			"Penkov",
			TestData.DateOfBirth,
			TestData.AddressModel(),
			"+359878435888",
			TestData.Iban);
	}
}
