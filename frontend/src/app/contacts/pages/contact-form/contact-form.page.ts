import {
  ChangeDetectionStrategy,
  Component,
  ElementRef,
  computed,
  effect,
  inject,
  input,
  signal,
} from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NonNullableFormBuilder, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Store } from '@ngrx/store';
import { ButtonModule } from 'primeng/button';
import { filter } from 'rxjs';

import { ContactFormFields } from '../../components/contact-form-fields/contact-form-fields';
import { createContactForm, toFormValue, toPayload } from '../../form/contact-form.model';
import {
  ContactsActions,
  selectFieldErrors,
  selectSaving,
  selectSelectedContact,
} from '../../store';

@Component({
  selector: 'app-contact-form-page',
  imports: [ReactiveFormsModule, ButtonModule, ContactFormFields],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './contact-form.page.html',
})
export class ContactFormPage {
  readonly id = input<string>();

  private readonly store = inject(Store);
  private readonly router = inject(Router);
  private readonly host = inject<ElementRef<HTMLElement>>(ElementRef);

  protected readonly form = createContactForm(inject(NonNullableFormBuilder));
  protected readonly saving = this.store.selectSignal(selectSaving);
  protected readonly serverErrors = this.store.selectSignal(selectFieldErrors);
  protected readonly submitAttempted = signal(false);

  private readonly selectedContact = this.store.selectSignal(selectSelectedContact);

  private readonly contact = computed(() => {
    const id = this.id();
    const contact = this.selectedContact();

    return id && contact?.id === id ? contact : null;
  });

  protected get isEdit(): boolean {
    return !!this.id();
  }

  constructor() {
    this.store.dispatch(ContactsActions.clearErrors());

    effect(() => {
      const id = this.id();
      if (id) {
        this.store.dispatch(ContactsActions.selectContact({ id }));
        this.store.dispatch(ContactsActions.loadContact({ id }));
      } else {
        this.store.dispatch(ContactsActions.selectContact({ id: null }));
      }
    });

    effect(() => {
      const contact = this.contact();
      if (contact && this.form.pristine) {
        this.form.patchValue(toFormValue(contact));
      }
    });

    this.form.valueChanges
      .pipe(
        filter(() => Object.keys(this.serverErrors()).length > 0),
        takeUntilDestroyed(),
      )
      .subscribe(() => this.store.dispatch(ContactsActions.clearErrors()));
  }

  protected submit(): void {
    this.submitAttempted.set(true);

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      this.focusFirstInvalidControl();
      return;
    }

    const payload = toPayload(this.form);
    const id = this.id();

    this.store.dispatch(
      id
        ? ContactsActions.updateContact({ id, payload })
        : ContactsActions.createContact({ payload }),
    );
  }

  protected cancel(): void {
    void this.router.navigate(['/contacts']);
  }

  private focusFirstInvalidControl(): void {
    this.host.nativeElement
      .querySelector<HTMLElement>('input.ng-invalid, .ng-invalid input')
      ?.focus();
  }
}
