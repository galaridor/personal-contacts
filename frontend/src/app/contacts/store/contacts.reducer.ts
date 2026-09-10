import { createFeature, createReducer, on } from '@ngrx/store';
import { Contact, DEFAULT_PAGE_SIZE } from '../../core/models';
import { ContactsActions } from './contacts.actions';

export interface ContactsState {
  contacts: Contact[];
  search: string | null;
  page: number;
  pageSize: number;
  totalCount: number;
  selectedContactId: string | null;
  selectedContact: Contact | null;
  loading: boolean;
  saving: boolean;
  error: string | null;
  fieldErrors: Record<string, string[]>;
}

export const initialState: ContactsState = {
  contacts: [],
  search: null,
  page: 1,
  pageSize: DEFAULT_PAGE_SIZE,
  totalCount: 0,
  selectedContactId: null,
  selectedContact: null,
  loading: false,
  saving: false,
  error: null,
  fieldErrors: {},
};

function replaceIfPresent(contacts: Contact[], contact: Contact): Contact[] {
  return contacts.some((candidate) => candidate.id === contact.id)
    ? contacts.map((candidate) => (candidate.id === contact.id ? contact : candidate))
    : contacts;
}

export const contactsFeature = createFeature({
  name: 'contacts',
  reducer: createReducer(
    initialState,

    on(ContactsActions.loadContacts, (state, { search }) => ({
      ...state,
      search: search ?? null,
      loading: true,
      error: null,
    })),
    on(ContactsActions.loadContactsSuccess, (state, { result }) => ({
      ...state,
      contacts: result.items,
      page: result.page,
      pageSize: result.pageSize,
      totalCount: result.totalCount,
      loading: false,
    })),
    on(ContactsActions.loadContactsFailure, (state, { error }) => ({
      ...state,
      loading: false,
      error: error.message,
    })),

    on(ContactsActions.loadContact, (state) => ({ ...state, loading: true, error: null })),
    on(ContactsActions.loadContactSuccess, (state, { contact }) => ({
      ...state,
      contacts: replaceIfPresent(state.contacts, contact),
      selectedContact: contact,
      selectedContactId: contact.id,
      loading: false,
    })),
    on(ContactsActions.loadContactFailure, (state, { error }) => ({
      ...state,
      loading: false,
      error: error.message,
    })),

    on(ContactsActions.createContact, ContactsActions.updateContact, (state) => ({
      ...state,
      saving: true,
      error: null,
      fieldErrors: {},
    })),
    on(ContactsActions.createContactSuccess, (state) => ({ ...state, saving: false })),
    on(ContactsActions.updateContactSuccess, (state, { contact }) => ({
      ...state,
      contacts: replaceIfPresent(state.contacts, contact),
      selectedContact: state.selectedContactId === contact.id ? contact : state.selectedContact,
      saving: false,
    })),
    on(
      ContactsActions.createContactFailure,
      ContactsActions.updateContactFailure,
      (state, { error }) => ({
        ...state,
        saving: false,
        error: error.message,
        fieldErrors: error.fieldErrors,
      }),
    ),

    on(ContactsActions.deleteContact, (state) => ({ ...state, saving: true, error: null })),
    on(ContactsActions.deleteContactSuccess, (state, { id }) => ({
      ...state,
      contacts: state.contacts.filter((contact) => contact.id !== id),
      totalCount: Math.max(0, state.totalCount - 1),
      selectedContactId: state.selectedContactId === id ? null : state.selectedContactId,
      selectedContact: state.selectedContact?.id === id ? null : state.selectedContact,
      saving: false,
    })),
    on(ContactsActions.deleteContactFailure, (state, { error }) => ({
      ...state,
      saving: false,
      error: error.message,
    })),

    on(ContactsActions.selectContact, (state, { id }) => ({
      ...state,
      selectedContactId: id,
      selectedContact: state.selectedContact?.id === id ? state.selectedContact : null,
    })),
    on(ContactsActions.clearErrors, (state) => ({ ...state, error: null, fieldErrors: {} })),
  ),
});
