import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';
import { toObservable, toSignal } from '@angular/core/rxjs-interop';
import { AbstractControl, ReactiveFormsModule } from '@angular/forms';
import { DatePickerModule } from 'primeng/datepicker';
import { InputTextModule } from 'primeng/inputtext';
import { switchMap } from 'rxjs';

import { ContactForm } from '../../form/contact-form.model';

const LABELS: Record<string, string> = {
  firstName: 'First name',
  surname: 'Surname',
  dateOfBirth: 'Date of birth',
  phoneNumber: 'Phone number',
  iban: 'IBAN',
  'address.street': 'Street',
  'address.houseNumber': 'House number',
  'address.postalCode': 'Postal code',
  'address.city': 'City',
  'address.country': 'Country',
};

@Component({
  selector: 'app-contact-form-fields',
  imports: [ReactiveFormsModule, DatePickerModule, InputTextModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './contact-form-fields.html',
})
export class ContactFormFields {
  readonly form = input.required<ContactForm>();

  readonly serverErrors = input<Record<string, string[]>>({});

  readonly submitAttempted = input(false);

  protected readonly today = new Date();

  private readonly formEvents = toSignal(
    toObservable(this.form).pipe(switchMap((form) => form.events)),
  );

  protected readonly messages = computed<Record<string, string>>(() => {
    this.formEvents();

    const form = this.form();
    const serverErrors = this.serverErrors();
    const submitAttempted = this.submitAttempted();
    const messages: Record<string, string> = {};

    for (const path of Object.keys(LABELS)) {
      const fromServer = serverErrors[path]?.[0];
      if (fromServer) {
        messages[path] = fromServer;
        continue;
      }

      const control = form.get(path);
      if (!control || control.valid) {
        continue;
      }

      if (submitAttempted || control.touched || control.dirty) {
        messages[path] = clientMessage(path, control);
      }
    }

    return messages;
  });
}

function clientMessage(path: string, control: AbstractControl): string {
  const label = LABELS[path];

  if (control.hasError('required')) {
    return `${label} is required.`;
  }

  if (control.hasError('future')) {
    return 'Date of birth cannot be in the future.';
  }

  if (control.hasError('pattern')) {
    switch (path) {
      case 'phoneNumber':
        return 'Enter a phone number using digits and the characters + - . ( ) / only.';
      case 'iban':
        return 'Enter an IBAN starting with a two-letter country code and two check digits.';
      case 'address.country':
        return "Country must be a two-letter country code, for example 'NL'.";
    }
  }

  return `${label} is not valid.`;
}
