import { provideHttpClient, withInterceptors } from '@angular/common/http';
import {
  ApplicationConfig,
  isDevMode,
  provideBrowserGlobalErrorListeners,
  provideZonelessChangeDetection,
} from '@angular/core';
import { provideRouter, withComponentInputBinding } from '@angular/router';
import { provideEffects } from '@ngrx/effects';
import { provideState, provideStore } from '@ngrx/store';
import { provideStoreDevtools } from '@ngrx/store-devtools';
import { providePrimeNG } from 'primeng/config';
import { ConfirmationService, MessageService } from 'primeng/api';
import Aura from '@primeuix/themes/aura';

import { appRoutes } from './app.routes';
import * as contactsEffects from './contacts/store/contacts.effects';
import { contactsFeature } from './contacts/store';
import { problemDetailsInterceptor } from './core/interceptors/problem-details.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZonelessChangeDetection(),
    provideRouter(appRoutes, withComponentInputBinding()),
    provideHttpClient(withInterceptors([problemDetailsInterceptor])),
    provideStore(),
    provideState(contactsFeature),
    provideEffects(contactsEffects),
    provideStoreDevtools({ maxAge: 25, logOnly: !isDevMode(), connectInZone: false }),

    providePrimeNG({
      theme: {
        preset: Aura,
        options: {
          darkModeSelector: '.app-dark',
          cssLayer: { name: 'primeng', order: 'theme, base, components, primeng, utilities' },
        },
      },
    }),
    MessageService,
    ConfirmationService,
  ],
};
