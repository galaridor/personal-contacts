using Contacts.Domain.Entities;
using Contacts.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Contacts.Infrastructure.Persistence.Configurations
{
	// Mapping is done with the Fluent API rather than attributes so the domain model has no persistence concerns
	internal sealed class ContactConfiguration : IEntityTypeConfiguration<Contact>
	{
		public void Configure(EntityTypeBuilder<Contact> builder)
		{
			builder.ToTable("contacts");

			builder.HasKey(contact => contact.Id);

			builder.Property(contact => contact.Id)
				.HasColumnName("id")
				.ValueGeneratedNever();

			builder.Property(contact => contact.FirstName)
				.HasColumnName("first_name")
				.HasMaxLength(Contact.MaxNameLength)
				.IsRequired();

			builder.Property(contact => contact.Surname)
				.HasColumnName("surname")
				.HasMaxLength(Contact.MaxNameLength)
				.IsRequired();

			builder.Property(contact => contact.DateOfBirth)
				.HasColumnName("date_of_birth")
				.HasColumnType("date")
				.IsRequired();

			builder.ComplexProperty(contact => contact.Address, address =>
			{
				address.Property(value => value.Street)
					.HasColumnName("address_street")
					.HasMaxLength(Address.MaxStreetLength)
					.IsRequired();

				address.Property(value => value.HouseNumber)
					.HasColumnName("address_house_number")
					.HasMaxLength(Address.MaxHouseNumberLength)
					.IsRequired();

				address.Property(value => value.PostalCode)
					.HasColumnName("address_postal_code")
					.HasMaxLength(Address.MaxPostalCodeLength)
					.IsRequired();

				address.Property(value => value.City)
					.HasColumnName("address_city")
					.HasMaxLength(Address.MaxCityLength)
					.IsRequired();

				address.Property(value => value.Country)
					.HasColumnName("address_country")
					.HasMaxLength(Address.CountryCodeLength)
					.IsFixedLength()
					.IsRequired();
			});

			builder.ComplexProperty(contact => contact.PhoneNumber, phoneNumber =>
			{
				phoneNumber.Property(value => value.Value)
					.HasColumnName("phone_number")
					.HasMaxLength(20)
					.IsRequired();
			});

			builder.ComplexProperty(contact => contact.Iban, iban =>
			{
				iban.Property(value => value.Value)
					.HasColumnName("iban")
					.HasMaxLength(34)
					.IsRequired();
			});

			builder.Property(contact => contact.CreatedAtUtc)
				.HasColumnName("created_at_utc")
				.IsRequired();

			builder.Property(contact => contact.UpdatedAtUtc)
				.HasColumnName("updated_at_utc")
				.IsRequired();

			builder.HasIndex(contact => new { contact.Surname, contact.FirstName })
				.HasDatabaseName("ix_contacts_surname_first_name");
		}
	}
}
