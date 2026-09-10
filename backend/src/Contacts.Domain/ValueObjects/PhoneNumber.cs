using Contacts.Domain.Exceptions;
using System.Text;

namespace Contacts.Domain.ValueObjects
{
	public sealed record PhoneNumber
	{
		private const int MinDigits = 7;
		private const int MaxDigits = 15;

		private PhoneNumber(string value)
		{
			Value = value;
		}

		// Digits only prefixed with "+" when an international
		public string Value { get; }

		public static PhoneNumber Create(string? input)
		{
			if (string.IsNullOrWhiteSpace(input))
			{
				throw new DomainException("Phone number is required.", nameof(PhoneNumber));
			}

			string raw = input.Trim();

			// A leading "00" is the same international prefix as "+".
			bool isInternational = raw.StartsWith('+') || raw.StartsWith("00", StringComparison.Ordinal);

			if (raw.StartsWith("00", StringComparison.Ordinal))
			{
				raw = raw[2..];
			}
			else if (raw.StartsWith('+'))
			{
				raw = raw[1..];
			}

			StringBuilder digits = new StringBuilder(raw.Length);
			foreach (char character in raw)
			{
				if (char.IsAsciiDigit(character))
				{
					digits.Append(character);
					continue;
				}

				// No meaning separators but allow them for user input
				if (character is ' ' or '-' or '.' or '(' or ')' or '/')
				{
					continue;
				}

				throw new DomainException(
					"Phone number may only contain digits, spaces and the characters + - . ( ) /.",
					nameof(PhoneNumber));
			}

			if (digits.Length is < MinDigits or > MaxDigits)
			{
				throw new DomainException(
					$"Phone number must contain between {MinDigits} and {MaxDigits} digits.",
					nameof(PhoneNumber));
			}

			string normalized = isInternational ? $"+{digits}" : digits.ToString();

			return new PhoneNumber(normalized);
		}

		public override string ToString() => Value;
	}
}