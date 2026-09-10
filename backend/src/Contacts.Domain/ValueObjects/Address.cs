using Contacts.Domain.Exceptions;

namespace Contacts.Domain.ValueObjects
{
	public sealed record Address
	{
		public const int MaxStreetLength = 200;
		public const int MaxHouseNumberLength = 20;
		public const int MaxPostalCodeLength = 20;
		public const int MaxCityLength = 100;
		public const int CountryCodeLength = 2;

		private Address(string street, string houseNumber, string postalCode, string city, string country)
		{
			Street = street;
			HouseNumber = houseNumber;
			PostalCode = postalCode;
			City = city;
			Country = country;
		}

		public string Street { get; }

		public string HouseNumber { get; }

		public string PostalCode { get; }

		public string City { get; }

		// 2-letter Country code
		public string Country { get; }

		public static Address Create(
			string? street,
			string? houseNumber,
			string? postalCode,
			string? city,
			string? country)
		{
			string validStreet = Ensure(street, MaxStreetLength, "Street", nameof(Street));
			string validHouseNumber = Ensure(houseNumber, MaxHouseNumberLength, "House number", nameof(HouseNumber));
			string validPostalCode = Ensure(postalCode, MaxPostalCodeLength, "Postal code", nameof(PostalCode));
			string validCity = Ensure(city, MaxCityLength, "City", nameof(City));
			string validCountry = EnsureCountry(country);

			return new Address(
				validStreet,
				validHouseNumber,
				validPostalCode,
				validCity,
				validCountry);
		}

		public override string ToString() =>
			$"{Street} {HouseNumber}, {PostalCode} {City}, {Country}";

		private static string Ensure(string? value, int maxLength, string label, string propertyName)
		{
			if (string.IsNullOrWhiteSpace(value))
			{
				throw new DomainException($"{label} is required.", propertyName);
			}

			if (value.Length > maxLength)
			{
				throw new DomainException($"{label} must be {maxLength} characters or fewer.", propertyName);
			}

			return value;
		}

		private static string EnsureCountry(string? country)
		{
			if (string.IsNullOrWhiteSpace(country))
			{
				throw new DomainException("Country is required.", nameof(Country));
			}

			if (country.Length != CountryCodeLength || !country.All(char.IsAsciiLetterUpper))
			{
				throw new DomainException(
					"Country must be a two-letter country code, for example 'BG'.",
					nameof(Country));
			}

			return country;
		}
	}
}
