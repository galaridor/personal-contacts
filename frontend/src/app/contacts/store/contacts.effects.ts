import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { concatLatestFrom, mapResponse } from '@ngrx/operators';
import { Store } from '@ngrx/store';
import { MessageService } from 'primeng/api';
import { concatMap, exhaustMap, map, switchMap, tap } from 'rxjs';
import { ContactsApiService } from '../../core/api/contacts-api.service';
import { ApiError, lastPage } from '../../core/models';
import { ContactsActions } from './contacts.actions';
import { selectCurrentPageRequest, selectTotalCount } from './contacts.selectors';

export const loadContacts$ = createEffect(
  (actions$ = inject(Actions), api = inject(ContactsApiService)) =>
    actions$.pipe(
      ofType(ContactsActions.loadContacts),
      switchMap(({ search, page, pageSize }) =>
        api.getPage({ search, page, pageSize }).pipe(
          mapResponse({
            next: (result) => ContactsActions.loadContactsSuccess({ result }),
            error: (error: ApiError) => ContactsActions.loadContactsFailure({ error }),
          }),
        ),
      ),
    ),
  { functional: true },
);

export const loadContact$ = createEffect(
  (actions$ = inject(Actions), api = inject(ContactsApiService)) =>
    actions$.pipe(
      ofType(ContactsActions.loadContact),
      switchMap(({ id }) =>
        api.getById(id).pipe(
          mapResponse({
            next: (contact) => ContactsActions.loadContactSuccess({ contact }),
            error: (error: ApiError) => ContactsActions.loadContactFailure({ error }),
          }),
        ),
      ),
    ),
  { functional: true },
);

export const createContact$ = createEffect(
  (actions$ = inject(Actions), api = inject(ContactsApiService)) =>
    actions$.pipe(
      ofType(ContactsActions.createContact),
      exhaustMap(({ payload }) =>
        api.create(payload).pipe(
          mapResponse({
            next: (contact) => ContactsActions.createContactSuccess({ contact }),
            error: (error: ApiError) => ContactsActions.createContactFailure({ error }),
          }),
        ),
      ),
    ),
  { functional: true },
);

export const updateContact$ = createEffect(
  (actions$ = inject(Actions), api = inject(ContactsApiService)) =>
    actions$.pipe(
      ofType(ContactsActions.updateContact),
      exhaustMap(({ id, payload }) =>
        api.update(id, payload).pipe(
          mapResponse({
            next: (contact) => ContactsActions.updateContactSuccess({ contact }),
            error: (error: ApiError) => ContactsActions.updateContactFailure({ error }),
          }),
        ),
      ),
    ),
  { functional: true },
);

export const deleteContact$ = createEffect(
  (actions$ = inject(Actions), api = inject(ContactsApiService)) =>
    actions$.pipe(
      ofType(ContactsActions.deleteContact),
      concatMap(({ id }) =>
        api.delete(id).pipe(
          mapResponse({
            next: () => ContactsActions.deleteContactSuccess({ id }),
            error: (error: ApiError) => ContactsActions.deleteContactFailure({ error }),
          }),
        ),
      ),
    ),
  { functional: true },
);

export const reloadPageAfterDelete$ = createEffect(
  (actions$ = inject(Actions), store = inject(Store)) =>
    actions$.pipe(
      ofType(ContactsActions.deleteContactSuccess),
      concatLatestFrom(() => [
        store.select(selectCurrentPageRequest),
        store.select(selectTotalCount),
      ]),
      map(([, request, remaining]) =>
        ContactsActions.loadContacts({
          ...request,
          page: Math.min(request.page, lastPage(remaining, request.pageSize)),
        }),
      ),
    ),
  { functional: true },
);

export const returnToListWhenContactMissing$ = createEffect(
  (actions$ = inject(Actions), router = inject(Router)) =>
    actions$.pipe(
      ofType(ContactsActions.loadContactFailure),
      tap(() => void router.navigate(['/contacts'])),
    ),
  { functional: true, dispatch: false },
);

export const returnToListAfterSave$ = createEffect(
  (actions$ = inject(Actions), router = inject(Router)) =>
    actions$.pipe(
      ofType(ContactsActions.createContactSuccess, ContactsActions.updateContactSuccess),
      tap(() => void router.navigate(['/contacts'])),
    ),
  { functional: true, dispatch: false },
);

export const notifySuccess$ = createEffect(
  (actions$ = inject(Actions), messages = inject(MessageService)) =>
    actions$.pipe(
      ofType(
        ContactsActions.createContactSuccess,
        ContactsActions.updateContactSuccess,
        ContactsActions.deleteContactSuccess,
      ),
      map((action) => {
        switch (action.type) {
          case ContactsActions.createContactSuccess.type:
            return 'Contact created.';
          case ContactsActions.updateContactSuccess.type:
            return 'Contact updated.';
          default:
            return 'Contact deleted.';
        }
      }),
      tap((detail) => messages.add({ severity: 'success', summary: 'Saved', detail, life: 4000 })),
    ),
  { functional: true, dispatch: false },
);

export const notifyFailure$ = createEffect(
  (actions$ = inject(Actions), messages = inject(MessageService)) =>
    actions$.pipe(
      ofType(
        ContactsActions.loadContactsFailure,
        ContactsActions.loadContactFailure,
        ContactsActions.createContactFailure,
        ContactsActions.updateContactFailure,
        ContactsActions.deleteContactFailure,
      ),
      tap(({ error }) =>
        messages.add({
          severity: 'error',
          summary: 'Something went wrong',
          detail: error.message,
          life: 6000,
        }),
      ),
    ),
  { functional: true, dispatch: false },
);
