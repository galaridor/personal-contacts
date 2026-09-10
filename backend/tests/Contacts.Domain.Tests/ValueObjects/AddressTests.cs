using Contacts.Domain.Exceptions;
using Contacts.Domain.ValueObjects;

namespace Contacts.Domain.Tests.ValueObjects
{
	public class AddressTests
	{
		[Fact]
		public void Create_stores_the_values_it_is_given()
		{
			Address address = Address.Create("Dimitar Ikonomov", "38", "5600", "Troyan", "BG");

			Assert.Equal("Dimitar Ikonomov", address.Street);
			Assert.Equal("38", address.HouseNumber);
			Assert.Equal("5600", address.PostalCode);
			Assert.Equal("Troyan", address.City);
			Assert.Equal("BG", address.Country);
		}

		[Theory]
		[InlineData(null, "123", "1015 CJ", "Troyan", "BG", "Street is required.")]
		[InlineData("Dimitar Ikonomov", " ", "1015 CJ", "Troyan", "BG", "House number is required.")]
		[InlineData("Dimitar Ikonomov", "38", "", "Troyan", "BG", "Postal code is required.")]
		[InlineData("Dimitar Ikonomov", "38", "5600", null, "BG", "City is required.")]
		[InlineData("Dimitar Ikonomov", "38", "5600", "Troyan", "", "Country is required.")]
		public void Create_rejects_missing_parts(
			string? street,
			string? houseNumber,
			string? postalCode,
			string? city,
			string? country,
			string expectedMessage)
		{
			Assert.Equal(
				expectedMessage,
				Assert.Throws<DomainException>(() => Address.Create(street, houseNumber, postalCode, city, country)).Message);
		}

		[Fact]
		public void Create_rejects_a_street_that_is_too_long()
		{
			string tooLong = new string('a', Address.MaxStreetLength + 1);

			Assert.Contains(
				"200 characters or fewer",
				Assert.Throws<DomainException>(() => Address.Create(tooLong, "38", "5600", "Troyan", "BG")).Message);
		}

		[Theory]
		[InlineData("bg")]
		[InlineData("Bg")]
		[InlineData("B1")]
		[InlineData("12")]
		[InlineData("@#")]
		public void Create_rejects_a_country_that_is_not_two_upper_case_letters(string country)
		{
			Assert.Contains(
				"two-letter country code",
				Assert.Throws<DomainException>(
					() => Address.Create("Dimitar Ikonomov", "38", "5600", "Troyan", country)).Message);
		}

		[Fact]
		public void Addresses_with_the_same_parts_are_equal()
		{
			Assert.Equal(
				Address.Create("Dimitar Ikonomov", "38", "5600", "Troyan", "BG"),
				Address.Create("Dimitar Ikonomov", "38", "5600", "Troyan", "BG"));
		}
	}
}
