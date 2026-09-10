import {
  AbstractControl,
  FormControl,
  FormGroup,
  NonNullableFormBuilder,
  ValidationErrors,
  Validators,
} from '@angular/forms';
import { Contact, ContactPayload } from '../../core/models';
import { fromIsoDate, toIsoDate } from '../../core/formatting/date-only';

export interface ContactFormValue {
  firstName: FormControl<string>;
  surname: FormControl<string>;
  dateOfBirth: FormControl<Date | null>;
  phoneNumber: FormControl<string>;
  iban: FormControl<string>;
  address: FormGroup<{
    street: FormControl<string>;
    houseNumber: FormControl<string>;
    postalCode: FormControl<string>;
    city: FormControl<string>;
    country: FormControl<string>;
  }>;
}

export type ContactForm = FormGroup<ContactFormValue>;

export type ContactFormRawValue = ReturnType<ContactForm['getRawValue']>;

const PHONE_PATTERN = /^\+?[\d\s\-.()/]{7,25}$/;

const IBAN_PATTERN = /^[A-Za-z]{2}\d{2}[\sA-Za-z0-9]{11,30}$/;

const COUNTRY_PATTERN = /^[A-Za-z]{2}$/;

export function createContactForm(builder: NonNullableFormBuilder): ContactForm {
  return builder.group<ContactFormValue>({
    firstName: builder.control('', Validators.required),
    surname: builder.control('', Validators.required),
    dateOfBirth: builder.control<Date | null>(null, [Validators.required, notInTheFuture]),
    phoneNumber: builder.control('', [Validators.required, Validators.pattern(PHONE_PATTERN)]),
    iban: builder.control('', [Validators.required, Validators.pattern(IBAN_PATTERN)]),
    address: builder.group({
      street: builder.control('', Validators.required),
      houseNumber: builder.control('', Validators.required),
      postalCode: builder.control('', Validators.required),
      city: builder.control('', Validators.required),
      country: builder.control('', [Validators.required, Validators.pattern(COUNTRY_PATTERN)]),
    }),
  });
}

function notInTheFuture(control: AbstractControl<Date | null>): ValidationErrors | null {
  if (!control.value) {
    return null;
  }

  const today = new Date();
  today.setHours(23, 59, 59, 999);

  return control.value > today ? { future: true } : null;
}

export function toFormValue(contact: Contact): ContactFormRawValue {
  return {
    firstName: contact.firstName,
    surname: contact.surname,
    dateOfBirth: fromIsoDate(contact.dateOfBirth),
    phoneNumber: contact.phoneNumber,
    iban: contact.iban,
    address: { ...contact.address },
  };
}

export function toPayload(form: ContactForm): ContactPayload {
  const value = form.getRawValue();

  return {
    firstName: value.firstName.trim(),
    surname: value.surname.trim(),
    dateOfBirth: toIsoDate(value.dateOfBirth),
    phoneNumber: value.phoneNumber.trim(),
    iban: value.iban.trim(),
    address: {
      street: value.address.street.trim(),
      houseNumber: value.address.houseNumber.trim(),
      postalCode: value.address.postalCode.trim(),
      city: value.address.city.trim(),
      country: value.address.country.trim().toUpperCase(),
    },
  };
}
