import { Routes } from '@angular/router';

export const appRoutes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'contacts' },
  {
    path: 'contacts',
    loadComponent: () =>
      import('./contacts/pages/contacts-list/contacts-list.page').then((m) => m.ContactsListPage),
    title: 'Contacts',
  },
  {
    path: 'contacts/new',
    loadComponent: () =>
      import('./contacts/pages/contact-form/contact-form.page').then((m) => m.ContactFormPage),
    title: 'New contact',
  },
  {
    path: 'contacts/:id',
    loadComponent: () =>
      import('./contacts/pages/contact-details/contact-details.page').then(
        (m) => m.ContactDetailsPage,
      ),
    title: 'Contact',
  },
  {
    path: 'contacts/:id/edit',
    loadComponent: () =>
      import('./contacts/pages/contact-form/contact-form.page').then((m) => m.ContactFormPage),
    title: 'Edit contact',
  },
  { path: '**', redirectTo: 'contacts' },
];
