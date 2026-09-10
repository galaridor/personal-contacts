import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';
import { DatePipe } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { TableLazyLoadEvent, TableModule } from 'primeng/table';
import { TooltipModule } from 'primeng/tooltip';

import { Contact, ContactsPageRequest } from '../../../core/models';
import { IbanPipe } from '../../../core/formatting/iban.pipe';

const PAGE_SIZE_OPTIONS = [10, 20, 50];

@Component({
  selector: 'app-contact-table',
  imports: [DatePipe, ButtonModule, TableModule, TooltipModule, IbanPipe],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './contact-table.html',
})
export class ContactTable {
  readonly contacts = input.required<Contact[]>();
  readonly loading = input(false);
  readonly totalRecords = input(0);
  readonly rows = input(PAGE_SIZE_OPTIONS[0]);
  readonly first = input(0);
  readonly pageChange = output<Pick<ContactsPageRequest, 'page' | 'pageSize'>>();
  readonly view = output<Contact>();
  readonly edit = output<Contact>();
  readonly remove = output<Contact>();

  protected readonly pageSizeOptions = PAGE_SIZE_OPTIONS;

  protected trackById = (_: number, contact: Contact) => contact.id;

  protected readonly asContact = (row: unknown): Contact => row as Contact;

  protected onLazyLoad(event: TableLazyLoadEvent): void {
    const pageSize = event.rows ?? this.rows();
    const firstRow = event.first ?? 0;

    this.pageChange.emit({ page: Math.floor(firstRow / pageSize) + 1, pageSize });
  }
}
