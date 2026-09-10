using Contacts.Application.Common.Abstractions;
using Contacts.Application.Common.Results;
using Contacts.Application.Contacts.Commands.CreateContact;
using Contacts.Application.Contacts.Models;
using Contacts.Domain.Entities;
using Contacts.Domain.Exceptions;
using FluentValidation;
using Moq;

namespace Contacts.Application.Tests.Commands
{
	public class CreateContactCommandHandlerTests
	{
		private readonly Mock<IContactRepository> _repository = new Mock<IContactRepository>();
		private readonly Mock<IUnitOfWork> _unitOfWork = new Mock<IUnitOfWork>();
		private readonly CreateContactCommandHandler _handler;

		public CreateContactCommandHandlerTests()
		{
			_handler = new CreateContactCommandHandler(
				_repository.Object,
				_unitOfWork.Object,
				new CreateContactCommandValidator(TestData.Clock),
				TestData.Clock);
		}

		[Fact]
		public async Task Adds_the_contact_and_commits_once()
		{
			Result<ContactDto> result = await _handler.HandleAsync(ValidCommand());

			Assert.True(result.IsSuccess);
			Assert.Equal("Milen", result.Value.FirstName);
			Assert.Equal(TestData.NowUtc, result.Value.CreatedAtUtc);

			_repository.Verify(
				repository => repository.Add(It.Is<Contact>(contact => contact.Surname == "Penkov")),
				Times.Once);
			_unitOfWork.Verify(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
		}

		[Fact]
		public async Task Trims_and_upper_cases_the_input_before_the_domain_sees_it()
		{
			CreateContactCommand command = ValidCommand() with
			{
				FirstName = "  Milen  ",
				Surname = "  Penkov  ",
				Address = new AddressModel("  Dimitar Ikonomov  ", " 38 ", " 5600 ", " Troyan ", " bg "),
			};

			Result<ContactDto> result = await _handler.HandleAsync(command);

			Assert.Equal("Milen", result.Value.FirstName);
			Assert.Equal("Penkov", result.Value.Surname);
			Assert.Equal("Dimitar Ikonomov", result.Value.Address.Street);
			Assert.Equal("38", result.Value.Address.HouseNumber);
			Assert.Equal("5600", result.Value.Address.PostalCode);
			Assert.Equal("Troyan", result.Value.Address.City);
			Assert.Equal("BG", result.Value.Address.Country);
		}

		[Fact]
		public async Task Returns_the_canonical_form_the_value_objects_produced()
		{
			CreateContactCommand command = ValidCommand() with
			{
				PhoneNumber = "00359 (87) 843-5888",
				Iban = "bg80 bnbG 9661 1020 3456 78",
			};

			Result<ContactDto> result = await _handler.HandleAsync(command);

			Assert.Equal("+359878435888", result.Value.PhoneNumber);
			Assert.Equal(TestData.Iban, result.Value.Iban);
		}

		[Fact]
		public async Task Rejects_a_name_that_is_only_whitespace()
		{
			CreateContactCommand command = ValidCommand() with { FirstName = "   " };

			ValidationException exception =
				await Assert.ThrowsAsync<ValidationException>(() => _handler.HandleAsync(command));

			Assert.Equal("First name is required.", exception.Errors.Single().ErrorMessage);
		}

		[Fact]
		public async Task Rejects_a_malformed_request_before_touching_the_database()
		{
			CreateContactCommand command = ValidCommand() with { FirstName = "", Iban = "" };

			ValidationException exception = await Assert.ThrowsAsync<ValidationException>(() => _handler.HandleAsync(command));

			Assert.Equal(
				["FirstName", "Iban"],
				exception.Errors.Select(failure => failure.PropertyName).Order());

			_repository.Verify(repository => repository.Add(It.IsAny<Contact>()), Times.Never);
			_unitOfWork.Verify(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
		}

		[Fact]
		public async Task Rejects_a_date_of_birth_in_the_future()
		{
			CreateContactCommand command = ValidCommand() with { DateOfBirth = DateOnly.FromDateTime(TestData.NowUtc.UtcDateTime).AddDays(1) };

			ValidationException exception = await Assert.ThrowsAsync<ValidationException>(() => _handler.HandleAsync(command));

			Assert.Equal("Date of birth cannot be in the future.", exception.Errors.Single().ErrorMessage);
		}

		[Fact]
		public async Task Rejects_a_missing_steet()
		{
			CreateContactCommand command = ValidCommand() with { Address = new AddressModel(string.Empty, "38", "5600", "Troyan", "BG") };

			ValidationException exception = await Assert.ThrowsAsync<ValidationException>(() => _handler.HandleAsync(command));

			Assert.Equal("Street is required.", exception.Errors.First().ErrorMessage);
		}

		[Fact]
		public async Task Rejects_a_missing_address_even_when_validation_does_not_run()
		{
			CreateContactCommandHandler handler = new CreateContactCommandHandler(
				_repository.Object,
				_unitOfWork.Object,
				new InlineValidator<CreateContactCommand>(),
				TestData.Clock);

			DomainException exception = await Assert.ThrowsAsync<DomainException>(
				() => handler.HandleAsync(ValidCommand() with { Address = new AddressModel("", "", "", "", "") }));

			Assert.Equal("Street is required.", exception.Message);

			_repository.Verify(repository => repository.Add(It.IsAny<Contact>()), Times.Never);
			_unitOfWork.Verify(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
		}

		private static CreateContactCommand ValidCommand() => new(
			"Milen",
			"Penkov",
			TestData.DateOfBirth,
			TestData.AddressModel(),
			"+359878435888",
			TestData.Iban);
	}
}
