import { ChangeDetectionStrategy, Component, effect, inject, input } from '@angular/core';
import { Router } from '@angular/router';
import { Store } from '@ngrx/store';
import { DatePipe } from '@angular/common';
import { ConfirmationService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { SkeletonModule } from 'primeng/skeleton';

import { IbanPipe } from '../../../core/formatting/iban.pipe';
import { ContactsActions, selectLoading, selectSelectedContact } from '../../store';

@Component({
  selector: 'app-contact-details-page',
  imports: [DatePipe, ButtonModule, SkeletonModule, IbanPipe],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './contact-details.page.html',
})
export class ContactDetailsPage {
  readonly id = input.required<string>();

  private readonly store = inject(Store);
  private readonly router = inject(Router);
  private readonly confirmation = inject(ConfirmationService);

  protected readonly loading = this.store.selectSignal(selectLoading);
  protected readonly contact = this.store.selectSignal(selectSelectedContact);

  constructor() {
    effect(() => {
      const id = this.id();
      this.store.dispatch(ContactsActions.selectContact({ id }));
      this.store.dispatch(ContactsActions.loadContact({ id }));
    });
  }

  protected back(): void {
    void this.router.navigate(['/contacts']);
  }

  protected edit(): void {
    void this.router.navigate(['/contacts', this.id(), 'edit']);
  }

  protected remove(): void {
    const contact = this.contact();
    if (!contact) {
      return;
    }

    this.confirmation.confirm({
      header: 'Delete contact',
      message: `Delete ${contact.firstName} ${contact.surname}? This cannot be undone.`,
      icon: 'pi pi-exclamation-triangle',
      acceptButtonProps: { label: 'Delete', severity: 'danger' },
      rejectButtonProps: { label: 'Cancel', severity: 'secondary', outlined: true },
      accept: () => {
        this.store.dispatch(ContactsActions.deleteContact({ id: contact.id }));
        void this.router.navigate(['/contacts']);
      },
    });
  }
}
