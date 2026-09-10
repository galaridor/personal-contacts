import { createActionGroup, emptyProps, props } from '@ngrx/store';

import {
  ApiError,
  Contact,
  ContactPayload,
  ContactsPageRequest,
  PagedResult,
} from '../../core/models';

export const ContactsActions = createActionGroup({
  source: 'Contacts',
  events: {
    'Load Contacts': props<ContactsPageRequest>(),
    'Load Contacts Success': props<{ result: PagedResult<Contact> }>(),
    'Load Contacts Failure': props<{ error: ApiError }>(),

    'Load Contact': props<{ id: string }>(),
    'Load Contact Success': props<{ contact: Contact }>(),
    'Load Contact Failure': props<{ error: ApiError }>(),

    'Create Contact': props<{ payload: ContactPayload }>(),
    'Create Contact Success': props<{ contact: Contact }>(),
    'Create Contact Failure': props<{ error: ApiError }>(),

    'Update Contact': props<{ id: string; payload: ContactPayload }>(),
    'Update Contact Success': props<{ contact: Contact }>(),
    'Update Contact Failure': props<{ error: ApiError }>(),

    'Delete Contact': props<{ id: string }>(),
    'Delete Contact Success': props<{ id: string }>(),
    'Delete Contact Failure': props<{ error: ApiError }>(),

    'Select Contact': props<{ id: string | null }>(),

    'Clear Errors': emptyProps(),
  },
});
