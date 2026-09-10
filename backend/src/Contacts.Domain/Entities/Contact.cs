using Contacts.Domain.Exceptions;
using Contacts.Domain.ValueObjects;

namespace Contacts.Domain.Entities
{
	public sealed class Contact
	{
		public const int MaxNameLength = 100;

		private Contact()
		{
			FirstName = null!;
			Surname = null!;
			Address = null!;
			PhoneNumber = null!;
			Iban = null!;
		}

		private Contact(
			Guid id,
			string firstName,
			string surname,
			DateOnly dateOfBirth,
			Address address,
			PhoneNumber phoneNumber,
			Iban iban,
			DateTimeOffset createdAtUtc)
		{
			Id = id;
			FirstName = firstName;
			Surname = surname;
			DateOfBirth = dateOfBirth;
			Address = address;
			PhoneNumber = phoneNumber;
			Iban = iban;
			CreatedAtUtc = createdAtUtc;
			UpdatedAtUtc = createdAtUtc;
		}

		public Guid Id { get; private set; }

		public string FirstName { get; private set; }

		public string Surname { get; private set; }

		public DateOnly DateOfBirth { get; private set; }

		public Address Address { get; private set; }

		public PhoneNumber PhoneNumber { get; private set; }

		public Iban Iban { get; private set; }

		public DateTimeOffset CreatedAtUtc { get; private set; }

		public DateTimeOffset UpdatedAtUtc { get; private set; }

		public static Contact Create(
			string? firstName,
			string? surname,
			DateOnly dateOfBirth,
			Address address,
			PhoneNumber phoneNumber,
			Iban iban,
			DateTimeOffset nowUtc)
		{
			ArgumentNullException.ThrowIfNull(address);
			ArgumentNullException.ThrowIfNull(phoneNumber);
			ArgumentNullException.ThrowIfNull(iban);

			return new Contact(
				Guid.CreateVersion7(),
				EnsureName(firstName, "First name", nameof(FirstName)),
				EnsureName(surname, "Surname", nameof(Surname)),
				EnsureDateOfBirthIsNotInTheFuture(dateOfBirth, nowUtc),
				address,
				phoneNumber,
				iban,
				nowUtc);
		}

		public void Update(
			string? firstName,
			string? surname,
			DateOnly dateOfBirth,
			Address address,
			PhoneNumber phoneNumber,
			Iban iban,
			DateTimeOffset nowUtc)
		{
			ArgumentNullException.ThrowIfNull(address);
			ArgumentNullException.ThrowIfNull(phoneNumber);
			ArgumentNullException.ThrowIfNull(iban);

			// Validate everything before mutating anything so a rejected update leaves the aggregate exactly as it was
			string newFirstName = EnsureName(firstName, "First name", nameof(FirstName));
			string newSurname = EnsureName(surname, "Surname", nameof(Surname));
			DateOnly newDateOfBirth = EnsureDateOfBirthIsNotInTheFuture(dateOfBirth, nowUtc);

			FirstName = newFirstName;
			Surname = newSurname;
			DateOfBirth = newDateOfBirth;
			Address = address;
			PhoneNumber = phoneNumber;
			Iban = iban;
			UpdatedAtUtc = nowUtc;
		}

		private static string EnsureName(string? value, string label, string propertyName)
		{
			if (string.IsNullOrWhiteSpace(value))
			{
				throw new DomainException($"{label} is required.", propertyName);
			}

			if (value.Length > MaxNameLength)
			{
				throw new DomainException($"{label} must be {MaxNameLength} characters or fewer.", propertyName);
			}

			return value;
		}

		private static DateOnly EnsureDateOfBirthIsNotInTheFuture(DateOnly dateOfBirth, DateTimeOffset nowUtc)
		{
			DateOnly today = DateOnly.FromDateTime(nowUtc.UtcDateTime);

			if (dateOfBirth > today)
			{
				throw new DomainException("Date of birth cannot be in the future.", nameof(DateOfBirth));
			}

			return dateOfBirth;
		}
	}
}
