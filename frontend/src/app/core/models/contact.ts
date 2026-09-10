export interface Address {
  street: string;
  houseNumber: string;
  postalCode: string;
  city: string;
  country: string;
}

export interface Contact {
  id: string;
  firstName: string;
  surname: string;
  dateOfBirth: string;
  address: Address;
  phoneNumber: string;
  iban: string;
  createdAtUtc: string;
  updatedAtUtc: string;
}

export type ContactPayload = Omit<Contact, 'id' | 'createdAtUtc' | 'updatedAtUtc'>;
