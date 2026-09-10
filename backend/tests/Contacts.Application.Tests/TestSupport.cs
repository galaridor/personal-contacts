using Contacts.Application.Contacts.Models;
using Contacts.Domain.Entities;
using Contacts.Domain.ValueObjects;

namespace Contacts.Application.Tests
{
	internal sealed class FixedTimeProvider(DateTimeOffset now) : TimeProvider
	{
		public override DateTimeOffset GetUtcNow() => now;
	}

	internal static class TestData
	{
		public static readonly DateTimeOffset NowUtc = new(2026, 9, 4, 12, 0, 0, TimeSpan.Zero);

		public static readonly DateOnly DateOfBirth = new(1997, 10, 07);

		public const string Iban = "BG80BNBG96611020345678";

		public static readonly TimeProvider Clock = new FixedTimeProvider(NowUtc);

		public static AddressModel AddressModel() => new("Dimitar Ikonomov", "38", "5600", "Troyan", "BG");

		public static Contact Contact(string firstName = "Milen", string surname = "Penkov") =>
			Domain.Entities.Contact.Create(
				firstName,
				surname,
				DateOfBirth,
				Address.Create("Dimitar Ikonomov", "38", "5600", "Troyan", "BG"),
				PhoneNumber.Create("+359878435888"),
				Domain.ValueObjects.Iban.Create(Iban),
				NowUtc);
	}
}
