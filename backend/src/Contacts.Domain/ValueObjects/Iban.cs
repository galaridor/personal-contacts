using Contacts.Domain.Exceptions;
using System.Text;

namespace Contacts.Domain.ValueObjects
{
	public sealed record Iban
	{
		private const int MinLength = 15;
		private const int MaxLength = 34;

		// Official IBAN lengths 
		// In production this belongs in configuration or a reference-data service so it can be updated without a redeploy
		private static readonly Dictionary<string, int> LengthByCountry = new(StringComparer.Ordinal)
		{
			["AD"] = 24, ["AE"] = 23, ["AL"] = 28, ["AT"] = 20, ["AZ"] = 28, ["BA"] = 20, ["BE"] = 16,
			["BG"] = 22, ["BH"] = 22, ["BI"] = 27, ["BR"] = 29, ["BY"] = 28, ["CH"] = 21, ["CR"] = 22,
			["CY"] = 28, ["CZ"] = 24, ["DE"] = 22, ["DJ"] = 27, ["DK"] = 18, ["DO"] = 28, ["EE"] = 20,
			["EG"] = 29, ["ES"] = 24, ["FI"] = 18, ["FK"] = 18, ["FO"] = 18, ["FR"] = 27, ["GB"] = 22,
			["GE"] = 22, ["GI"] = 23, ["GL"] = 18, ["GR"] = 27, ["GT"] = 28, ["HN"] = 28, ["HR"] = 21,
			["HU"] = 28, ["IE"] = 22, ["IL"] = 23, ["IQ"] = 23, ["IS"] = 26, ["IT"] = 27, ["JO"] = 30,
			["KW"] = 30, ["KZ"] = 20, ["LB"] = 28, ["LC"] = 32, ["LI"] = 21, ["LT"] = 20, ["LU"] = 20,
			["LV"] = 21, ["LY"] = 25, ["MC"] = 27, ["MD"] = 24, ["ME"] = 22, ["MK"] = 19, ["MN"] = 20,
			["MR"] = 27, ["MT"] = 31, ["MU"] = 30, ["NI"] = 28, ["NL"] = 18, ["NO"] = 15, ["OM"] = 23,
			["PK"] = 24, ["PL"] = 28, ["PS"] = 29, ["PT"] = 25, ["QA"] = 29, ["RO"] = 24, ["RS"] = 22,
			["RU"] = 33, ["SA"] = 24, ["SC"] = 31, ["SD"] = 18, ["SE"] = 24, ["SI"] = 19, ["SK"] = 24,
			["SM"] = 27, ["SO"] = 23, ["ST"] = 25, ["SV"] = 28, ["TL"] = 23, ["TN"] = 24, ["TR"] = 26,
			["UA"] = 29, ["VA"] = 22, ["VG"] = 24, ["XK"] = 20, ["YE"] = 30,
		};

		private Iban(string value)
		{
			Value = value;
		}

		// Upper case, no spaces
		public string Value { get; }

		public static Iban Create(string? input)
		{
			if (string.IsNullOrWhiteSpace(input))
			{
				throw new DomainException("IBAN is required.", nameof(Iban));
			}

			string normalized = Normalize(input);

			if (normalized.Length is < MinLength or > MaxLength)
			{
				throw new DomainException(
					$"IBAN must be between {MinLength} and {MaxLength} characters.",
					nameof(Iban));
			}

			if (!char.IsAsciiLetterUpper(normalized[0]) ||
				!char.IsAsciiLetterUpper(normalized[1]) ||
				!char.IsAsciiDigit(normalized[2]) ||
				!char.IsAsciiDigit(normalized[3]))
			{
				throw new DomainException(
					"IBAN must start with a two-letter country code followed by two check digits.",
					nameof(Iban));
			}

			if (!normalized.All(char.IsAsciiLetterOrDigit))
			{
				throw new DomainException("IBAN may only contain letters and digits.", nameof(Iban));
			}

			string countryCode = normalized[..2];

			if (!LengthByCountry.TryGetValue(countryCode, out int expectedLength))
			{
				throw new DomainException($"'{countryCode}' is not a recognised IBAN country code.", nameof(Iban));
			}

			if (normalized.Length != expectedLength)
			{
				throw new DomainException(
					$"An IBAN for {countryCode} must be {expectedLength} characters long.",
					nameof(Iban));
			}

			return new Iban(normalized);
		}

		public override string ToString() => Value;

		private static string Normalize(string input)
		{
			StringBuilder builder = new StringBuilder(input.Length);

			foreach (char character in input)
			{
				if (char.IsWhiteSpace(character))
				{
					continue;
				}

				builder.Append(char.ToUpperInvariant(character));
			}

			return builder.ToString();
		}
	}
}
