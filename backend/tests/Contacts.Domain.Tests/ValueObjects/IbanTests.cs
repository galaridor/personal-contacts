using Contacts.Domain.Exceptions;
using Contacts.Domain.ValueObjects;

namespace Contacts.Domain.Tests.ValueObjects
{
	public class IbanTests
	{
		[Theory]
		[InlineData("NL91ABNA0417164300")]   // Netherlands
		[InlineData("GB29NWBK60161331926819")] // United Kingdom
		[InlineData("DE89370400440532013000")] // Germany
		[InlineData("BE68539007547034")]       // Belgium
		[InlineData("FR1420041010050500013M02606")] // France
		public void Create_accepts_known_valid_ibans(string value)
		{
			Iban iban = Iban.Create(value);

			Assert.Equal(value, iban.Value);
		}

		[Theory]
		[InlineData("nl91 abna 0417 1643 00", "NL91ABNA0417164300")]
		[InlineData("  GB29 NWBK 6016 1331 9268 19  ", "GB29NWBK60161331926819")]
		public void Create_normalizes_case_and_whitespace(string input, string expected)
		{
			Assert.Equal(expected, Iban.Create(input).Value);
		}

		[Theory]
		[InlineData(null)]
		[InlineData("")]
		[InlineData("   ")]
		public void Create_rejects_a_blank_iban(string? value)
		{
			Assert.Equal("IBAN is required.", Assert.Throws<DomainException>(() => Iban.Create(value)).Message);
		}

		[Fact]
		public void Create_rejects_an_unknown_country()
		{
			// ZZ is not in the IBAN registry so its length cannot be checked
			Assert.Contains(
				"not a recognised IBAN country code",
				Assert.Throws<DomainException>(() => Iban.Create("ZZ68539007547034")).Message);
		}

		[Fact]
		public void Create_rejects_a_correct_country_with_the_wrong_length()
		{
			Assert.Equal(
				"An IBAN for NL must be 18 characters long.",
				Assert.Throws<DomainException>(() => Iban.Create("NL91ABNA04171643")).Message);
		}

		[Fact]
		public void Create_rejects_a_malformed_prefix()
		{
			Assert.Contains(
				"two-letter country code",
				Assert.Throws<DomainException>(() => Iban.Create("1234ABNA0417164300")).Message);
		}

		[Fact]
		public void Create_rejects_punctuation()
		{
			Assert.Contains(
				"letters and digits",
				Assert.Throws<DomainException>(() => Iban.Create("NL91-ABNA-0417-1643-00")).Message);
		}

		[Fact]
		public void Two_ibans_with_the_same_normalized_value_are_equal()
		{
			Assert.Equal(Iban.Create("NL91ABNA0417164300"), Iban.Create("nl91 abna 0417 1643 00"));
		}
	}
}
