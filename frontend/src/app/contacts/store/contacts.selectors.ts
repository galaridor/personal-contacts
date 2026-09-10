import { createSelector } from '@ngrx/store';
import { contactsFeature } from './contacts.reducer';

export const {
  selectContacts,
  selectSearch,
  selectPage,
  selectPageSize,
  selectTotalCount,
  selectSelectedContactId,
  selectSelectedContact,
  selectLoading,
  selectSaving,
  selectError,
  selectFieldErrors,
} = contactsFeature;

export const selectFirstRow = createSelector(
  selectPage,
  selectPageSize,
  (page, pageSize) => (page - 1) * pageSize,
);

export const selectCurrentPageRequest = createSelector(
  selectSearch,
  selectPage,
  selectPageSize,
  (search, page, pageSize) => ({ search: search ?? undefined, page, pageSize }),
);

export const selectIsEmpty = createSelector(
  selectTotalCount,
  selectLoading,
  selectError,
  (totalCount, loading, error) => !loading && !error && totalCount === 0,
);
