import { ChangeDetectionStrategy, Component, inject, linkedSignal } from '@angular/core';
import { takeUntilDestroyed, toObservable } from '@angular/core/rxjs-interop';
import { Router } from '@angular/router';
import { Store } from '@ngrx/store';
import { ConfirmationService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { InputTextModule } from 'primeng/inputtext';
import { MessageModule } from 'primeng/message';
import { debounceTime, distinctUntilChanged, map, skip } from 'rxjs';

import { Contact, ContactsPageRequest } from '../../../core/models';
import { ContactTable } from '../../components/contact-table/contact-table';
import {
  ContactsActions,
  selectContacts,
  selectCurrentPageRequest,
  selectError,
  selectFirstRow,
  selectIsEmpty,
  selectLoading,
  selectPageSize,
  selectSearch,
  selectTotalCount,
} from '../../store';

const SEARCH_DEBOUNCE_MS = 250;

@Component({
  selector: 'app-contacts-list-page',
  imports: [
    ButtonModule,
    IconFieldModule,
    InputIconModule,
    InputTextModule,
    MessageModule,
    ContactTable,
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './contacts-list.page.html',
})
export class ContactsListPage {
  private readonly store = inject(Store);
  private readonly router = inject(Router);
  private readonly confirmation = inject(ConfirmationService);

  protected readonly contacts = this.store.selectSignal(selectContacts);
  protected readonly loading = this.store.selectSignal(selectLoading);
  protected readonly isEmpty = this.store.selectSignal(selectIsEmpty);
  protected readonly totalRecords = this.store.selectSignal(selectTotalCount);
  protected readonly pageSize = this.store.selectSignal(selectPageSize);
  protected readonly firstRow = this.store.selectSignal(selectFirstRow);
  protected readonly error = this.store.selectSignal(selectError);

  private readonly storeSearch = this.store.selectSignal(selectSearch);
  private readonly currentRequest = this.store.selectSignal(selectCurrentPageRequest);

  protected readonly search = linkedSignal(() => this.storeSearch() ?? '');

  constructor() {
    this.load(this.currentRequest());

    toObservable(this.search)
      .pipe(
        // toObservable replays the current value which the load above has already covered
        skip(1),
        debounceTime(SEARCH_DEBOUNCE_MS),
        map((search) => search.trim()),
        distinctUntilChanged(),
        takeUntilDestroyed(),
      )
      .subscribe((search) =>
        this.load({ search: search || undefined, page: 1, pageSize: this.pageSize() }),
      );
  }

  protected onSearch(term: string): void {
    this.search.set(term);
  }

  protected onPageChange(page: Pick<ContactsPageRequest, 'page' | 'pageSize'>): void {
    this.load({ ...page, search: this.search().trim() || undefined });
  }

  protected retry(): void {
    this.load(this.currentRequest());
  }

  protected addContact(): void {
    void this.router.navigate(['/contacts/new']);
  }

  protected view(contact: Contact): void {
    void this.router.navigate(['/contacts', contact.id]);
  }

  protected edit(contact: Contact): void {
    void this.router.navigate(['/contacts', contact.id, 'edit']);
  }

  protected remove(contact: Contact): void {
    this.confirmation.confirm({
      header: 'Delete contact',
      message: `Delete ${contact.firstName} ${contact.surname}? This cannot be undone.`,
      icon: 'pi pi-exclamation-triangle',
      acceptButtonProps: { label: 'Delete', severity: 'danger' },
      rejectButtonProps: { label: 'Cancel', severity: 'secondary', outlined: true },
      accept: () => this.store.dispatch(ContactsActions.deleteContact({ id: contact.id })),
    });
  }

  private load(request: ContactsPageRequest): void {
    this.store.dispatch(ContactsActions.loadContacts(request));
  }
}
