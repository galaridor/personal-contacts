using Contacts.Domain.Exceptions;
using Contacts.Domain.ValueObjects;

namespace Contacts.Domain.Tests.ValueObjects
{
	public class PhoneNumberTests
	{
		[Theory]
		[InlineData("+31 20 123 4567", "+31201234567")]
		[InlineData("+31-20-123-4567", "+31201234567")]
		[InlineData("+31 (20) 123 4567", "+31201234567")]
		[InlineData("0031 20 123 4567", "+31201234567")]
		[InlineData("020 123 4567", "0201234567")]
		public void Create_normalizes_separators_and_international_prefixes(string input, string expected)
		{
			Assert.Equal(expected, PhoneNumber.Create(input).Value);
		}

		[Theory]
		[InlineData(null)]
		[InlineData("")]
		[InlineData("  ")]
		public void Create_rejects_a_blank_number(string? input)
		{
			Assert.Equal(
				"Phone number is required.",
				Assert.Throws<DomainException>(() => PhoneNumber.Create(input)).Message);
		}

		[Theory]
		[InlineData("12345")]
		[InlineData("+3120123456789012")]
		public void Create_rejects_an_implausible_length(string input)
		{
			Assert.Contains(
				"between 7 and 15 digits",
				Assert.Throws<DomainException>(() => PhoneNumber.Create(input)).Message);
		}

		[Theory]
		[InlineData("020 123 CALL")]
		[InlineData("+31#201234567")]
		public void Create_rejects_characters_that_are_not_part_of_a_phone_number(string input)
		{
			Assert.Contains(
				"may only contain digits",
				Assert.Throws<DomainException>(() => PhoneNumber.Create(input)).Message);
		}

		[Fact]
		public void Numbers_typed_differently_but_meaning_the_same_are_equal()
		{
			Assert.Equal(PhoneNumber.Create("+31201234567"), PhoneNumber.Create("0031 (20) 123-4567"));
		}
	}
}
