using Contacts.Domain.ValueObjects;

namespace Contacts.Domain.Tests
{

	internal static class TestData
	{
		public static readonly DateTimeOffset NowUtc = new(2026, 9, 4, 12, 0, 0, TimeSpan.Zero);

		public const string ValidIban = "BG80BNBG96611020345678";

		public static Address Address() => Domain.ValueObjects.Address.Create("Dimitar Ikonomov", "38", "5600", "Troyan", "BG");

		public static PhoneNumber Phone() => PhoneNumber.Create("+359878435888");

		public static Iban Iban() => Domain.ValueObjects.Iban.Create(ValidIban);
	}
}