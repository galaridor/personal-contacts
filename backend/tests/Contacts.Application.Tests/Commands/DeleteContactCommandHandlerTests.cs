using Contacts.Application.Common.Abstractions;
using Contacts.Application.Common.Results;
using Contacts.Application.Contacts.Commands.DeleteContact;
using Contacts.Domain.Entities;
using Moq;

namespace Contacts.Application.Tests.Commands
{
	public class DeleteContactCommandHandlerTests
	{
		private readonly Mock<IContactRepository> _repository = new Mock<IContactRepository>();
		private readonly Mock<IUnitOfWork> _unitOfWork = new Mock<IUnitOfWork>();
		private readonly DeleteContactCommandHandler _handler;

		public DeleteContactCommandHandlerTests()
		{
			_handler = new DeleteContactCommandHandler(_repository.Object, _unitOfWork.Object);
		}

		[Fact]
		public async Task Removes_the_contact_and_commits()
		{
			Contact contact = TestData.Contact();
			_repository
				.Setup(repository => repository.GetByIdAsync(contact.Id, It.IsAny<CancellationToken>()))
				.ReturnsAsync(contact);

			Result result = await _handler.HandleAsync(new DeleteContactCommand(contact.Id));

			Assert.True(result.IsSuccess);
			_repository.Verify(repository => repository.Remove(contact), Times.Once);
			_unitOfWork.Verify(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
		}

		[Fact]
		public async Task Reports_an_unknown_contact_as_a_failed_result()
		{
			Guid id = Guid.NewGuid();
			_repository
				.Setup(repository => repository.GetByIdAsync(id, It.IsAny<CancellationToken>()))
				.ReturnsAsync((Contact?)null);

			Result result = await _handler.HandleAsync(new DeleteContactCommand(id));

			Assert.True(result.IsFailure);
			Assert.Equal("contact.not_found", result.Error.Code);
			_repository.Verify(repository => repository.Remove(It.IsAny<Contact>()), Times.Never);
			_unitOfWork.Verify(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
		}
	}
}
