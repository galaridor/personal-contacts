using Contacts.Domain.Entities;
using Contacts.Domain.Exceptions;
using Contacts.Domain.ValueObjects;

namespace Contacts.Domain.Tests.Entities
{
	public class ContactTests
	{
		private static readonly DateOnly DateOfBirth = new(1997, 10, 07);

		[Fact]
		public void Create_produces_a_valid_contact()
		{
			Contact contact = CreateContact();

			Assert.NotEqual(Guid.Empty, contact.Id);
			Assert.Equal("Milen", contact.FirstName);
			Assert.Equal("Penkov", contact.Surname);
			Assert.Equal(DateOfBirth, contact.DateOfBirth);
			Assert.Equal("Troyan", contact.Address.City);
			Assert.Equal("+359878435888", contact.PhoneNumber.Value);
			Assert.Equal(TestData.ValidIban, contact.Iban.Value);
			Assert.Equal(TestData.NowUtc, contact.CreatedAtUtc);
			Assert.Equal(TestData.NowUtc, contact.UpdatedAtUtc);
		}

		[Fact]
		public void Create_stores_the_names_it_is_given()
		{
			Contact contact = CreateContact(firstName: "Milen", surname: "Penkov");

			Assert.Equal("Milen", contact.FirstName);
			Assert.Equal("Penkov", contact.Surname);
		}

		[Theory]
		[InlineData(null)]
		[InlineData("")]
		[InlineData("   ")]
		public void Create_rejects_a_blank_first_name(string? firstName)
		{
			DomainException exception = Assert.Throws<DomainException>(() => CreateContact(firstName: firstName));

			Assert.Equal("First name is required.", exception.Message);
			Assert.Equal("FirstName", exception.PropertyName);
		}

		[Theory]
		[InlineData(null)]
		[InlineData("")]
		[InlineData("   ")]
		public void Create_rejects_a_blank_surname(string? surname)
		{
			DomainException exception = Assert.Throws<DomainException>(() => CreateContact(surname: surname));

			Assert.Equal("Surname is required.", exception.Message);
			Assert.Equal("Surname", exception.PropertyName);
		}

		[Fact]
		public void Create_rejects_a_name_that_is_too_long()
		{
			string tooLong = new string('a', Contact.MaxNameLength + 1);

			Assert.Contains(
				"100 characters or fewer",
				Assert.Throws<DomainException>(() => CreateContact(firstName: tooLong)).Message);
		}

		[Fact]
		public void Create_rejects_a_date_of_birth_in_the_future()
		{
			DateOnly tomorrow = DateOnly.FromDateTime(TestData.NowUtc.UtcDateTime).AddDays(1);

			DomainException exception = Assert.Throws<DomainException>(() => CreateContact(dateOfBirth: tomorrow));

			Assert.Equal("Date of birth cannot be in the future.", exception.Message);
			Assert.Equal("DateOfBirth", exception.PropertyName);
		}

		[Fact]
		public void Create_accepts_a_date_of_birth_of_today()
		{
			DateOnly today = DateOnly.FromDateTime(TestData.NowUtc.UtcDateTime);

			Assert.Equal(today, CreateContact(dateOfBirth: today).DateOfBirth);
		}

		[Fact]
		public void Create_accepts_a_historical_date_of_birth()
		{
			Assert.Equal(new DateOnly(1815, 12, 10), CreateContact(dateOfBirth: new DateOnly(1815, 12, 10)).DateOfBirth);
		}

		[Fact]
		public void Update_replaces_every_field_and_stamps_the_update_time()
		{
			Contact contact = CreateContact();
			DateTimeOffset later = TestData.NowUtc.AddDays(1);
			Address newAddress = Address.Create("Damrak", "1", "1012 LG", "Amsterdam", "NL");

			contact.Update(
				"Grace",
				"Hopper",
				new DateOnly(1906, 12, 9),
				newAddress,
				PhoneNumber.Create("+31207654321"),
				Iban.Create("DE89370400440532013000"),
				later);

			Assert.Equal("Grace", contact.FirstName);
			Assert.Equal("Hopper", contact.Surname);
			Assert.Equal(new DateOnly(1906, 12, 9), contact.DateOfBirth);
			Assert.Equal(newAddress, contact.Address);
			Assert.Equal("+31207654321", contact.PhoneNumber.Value);
			Assert.Equal("DE89370400440532013000", contact.Iban.Value);
			Assert.Equal(later, contact.UpdatedAtUtc);
			Assert.Equal(TestData.NowUtc, contact.CreatedAtUtc);
		}

		[Fact]
		public void Update_leaves_the_contact_untouched_when_it_is_rejected()
		{
			Contact contact = CreateContact();
			DateTimeOffset later = TestData.NowUtc.AddDays(1);
			Address newAddress = Address.Create("Damrak", "1", "1012 LG", "Amsterdam", "NL");

			Assert.Throws<DomainException>(() => contact.Update(
				firstName: "   ",
				surname: "Hopper",
				new DateOnly(1906, 12, 9),
				newAddress,
				PhoneNumber.Create("+31207654321"),
				Iban.Create("DE89370400440532013000"),
				later));

			Assert.Equal("Milen", contact.FirstName);
			Assert.Equal("Penkov", contact.Surname);
			Assert.Equal("Troyan", contact.Address.City);
			Assert.Equal("Dimitar Ikonomov", contact.Address.Street);
			Assert.Equal(TestData.NowUtc, contact.UpdatedAtUtc);
		}

		[Fact]
		public void Update_normalizes_the_value_objects_it_is_given()
		{
			Contact contact = CreateContact();

			contact.Update(
				"Grace",
				"Hopper",
				new DateOnly(1906, 12, 9),
				TestData.Address(),
				PhoneNumber.Create("0031 (20) 765-4321"),
				Iban.Create("de89 3704 0044 0532 0130 00"),
				TestData.NowUtc.AddHours(1));

			Assert.Equal("Grace", contact.FirstName);
			Assert.Equal("+31207654321", contact.PhoneNumber.Value);
			Assert.Equal("DE89370400440532013000", contact.Iban.Value);
		}

		[Fact]
		public void Update_rejects_a_null_value_object()
		{
			Contact contact = CreateContact();

			Assert.Throws<ArgumentNullException>(() => contact.Update(
				"Ada",
				"Lovelace",
				DateOfBirth,
				null!,
				TestData.Phone(),
				TestData.Iban(),
				TestData.NowUtc));
		}

		private static Contact CreateContact(
			string? firstName = "Milen",
			string? surname = "Penkov",
			DateOnly? dateOfBirth = null) =>
			Contact.Create(
				firstName,
				surname,
				dateOfBirth ?? DateOfBirth,
				TestData.Address(),
				TestData.Phone(),
				TestData.Iban(),
				TestData.NowUtc);
	}
}
