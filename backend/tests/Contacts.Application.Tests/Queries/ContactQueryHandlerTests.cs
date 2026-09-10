using Contacts.Application.Common.Abstractions;
using Contacts.Application.Common.Models;
using Contacts.Application.Common.Results;
using Contacts.Application.Contacts.Mapping;
using Contacts.Application.Contacts.Models;
using Contacts.Application.Contacts.Queries.GetContact;
using Contacts.Application.Contacts.Queries.GetContacts;
using FluentValidation;
using Moq;

namespace Contacts.Application.Tests.Queries
{
	public class ContactQueryHandlerTests
	{
		private readonly Mock<IContactQueries> _queries = new Mock<IContactQueries>();

		private readonly GetContactsQueryHandler _getContacts;

		public ContactQueryHandlerTests()
		{
			_getContacts = new GetContactsQueryHandler(_queries.Object, new GetContactsQueryValidator());
		}

		[Fact]
		public async Task GetContacts_returns_what_the_read_side_produced()
		{
			List<ContactDto> contacts = new List<ContactDto> { TestData.Contact().ToDto(), TestData.Contact("Grace", "Hopper").ToDto() };
			ListReturns(new PagedResult<ContactDto>(contacts, 1, 10, 2));

			Result<PagedResult<ContactDto>> result = await _getContacts.HandleAsync(new GetContactsQuery());

			Assert.True(result.IsSuccess);
			Assert.Equal(2, result.Value.Items.Count);
			Assert.Equal(2, result.Value.TotalCount);
		}

		[Fact]
		public async Task GetContacts_asks_for_the_first_page_when_nothing_is_specified()
		{
			ListReturns(new PagedResult<ContactDto>([], Paging.DefaultPage, Paging.DefaultPageSize, 0));

			await _getContacts.HandleAsync(new GetContactsQuery());

			_queries.Verify(
				queries => queries.ListAsync(
					null,
					Paging.DefaultPage,
					Paging.DefaultPageSize,
					It.IsAny<CancellationToken>()),
				Times.Once);
		}

		[Fact]
		public async Task GetContacts_passes_the_search_term_and_the_requested_page_to_the_read_side()
		{
			ListReturns(new PagedResult<ContactDto>([], 3, 25, 61));

			await _getContacts.HandleAsync(new GetContactsQuery("hop", Page: 3, PageSize: 25));

			_queries.Verify(
				queries => queries.ListAsync("hop", 3, 25, It.IsAny<CancellationToken>()),
				Times.Once);
		}

		[Fact]
		public async Task GetContacts_reports_the_paging_metadata_of_the_page_it_returned()
		{
			ListReturns(new PagedResult<ContactDto>([TestData.Contact().ToDto()], 3, 25, 61));

			Result<PagedResult<ContactDto>> result = await _getContacts.HandleAsync(
				new GetContactsQuery(Page: 3, PageSize: 25));

			Assert.Equal(3, result.Value.Page);
			Assert.Equal(25, result.Value.PageSize);
			Assert.Equal(61, result.Value.TotalCount);
		}

		[Fact]
		public async Task GetContacts_on_an_empty_database_succeeds_with_an_empty_page()
		{
			ListReturns(new PagedResult<ContactDto>([], 1, 10, 0));

			Result<PagedResult<ContactDto>> result = await _getContacts.HandleAsync(new GetContactsQuery());

			Assert.True(result.IsSuccess);
			Assert.Empty(result.Value.Items);
			Assert.Equal(0, result.Value.TotalCount);
		}

		[Theory]
		[InlineData(0, 10)]
		[InlineData(-1, 10)]
		[InlineData(1, 0)]
		[InlineData(1, Paging.MaxPageSize + 1)]
		public async Task GetContacts_rejects_paging_arguments_that_make_no_sense(int page, int pageSize)
		{
			await Assert.ThrowsAsync<ValidationException>(
				() => _getContacts.HandleAsync(new GetContactsQuery(Page: page, PageSize: pageSize)));

			_queries.Verify(
				queries => queries.ListAsync(
					It.IsAny<string?>(),
					It.IsAny<int>(),
					It.IsAny<int>(),
					It.IsAny<CancellationToken>()),
				Times.Never);
		}

		[Fact]
		public async Task GetContact_returns_the_contact()
		{
			ContactDto contact = TestData.Contact().ToDto();
			_queries
				.Setup(queries => queries.GetByIdAsync(contact.Id, It.IsAny<CancellationToken>()))
				.ReturnsAsync(contact);

			Result<ContactDto> result = await new GetContactQueryHandler(_queries.Object).HandleAsync(new GetContactQuery(contact.Id));

			Assert.True(result.IsSuccess);
			Assert.Equal("Milen", result.Value.FirstName);
			Assert.Equal("Penkov", result.Value.Surname);
		}

		[Fact]
		public async Task GetContact_reports_an_unknown_id_as_a_failed_result()
		{
			Guid id = Guid.NewGuid();
			_queries
				.Setup(queries => queries.GetByIdAsync(id, It.IsAny<CancellationToken>()))
				.ReturnsAsync((ContactDto?)null);

			Result<ContactDto> result = await new GetContactQueryHandler(_queries.Object).HandleAsync(new GetContactQuery(id));

			Assert.True(result.IsFailure);
			Assert.Equal("contact.not_found", result.Error.Code);
			Assert.Equal(Common.Results.ErrorType.NotFound, result.Error.Type);
			Assert.Throws<InvalidOperationException>(() => result.Value);
		}

		private void ListReturns(PagedResult<ContactDto> page) => _queries
			.Setup(queries => queries.ListAsync(
				It.IsAny<string?>(),
				It.IsAny<int>(),
				It.IsAny<int>(),
				It.IsAny<CancellationToken>()))
			.ReturnsAsync(page);
	}
}
