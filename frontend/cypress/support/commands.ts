import { ContactPayload } from '../../src/app/core/models';

const API = '/api/contacts';

export const contactFixture = (overrides: Partial<ContactPayload> = {}): ContactPayload => ({
  firstName: 'Ada',
  surname: 'Lovelace',
  dateOfBirth: '1990-05-20',
  phoneNumber: '+31 20 123 4567',
  iban: 'NL91 ABNA 0417 1643 00',
  address: {
    street: 'Keizersgracht',
    houseNumber: '123',
    postalCode: '1015 CJ',
    city: 'Amsterdam',
    country: 'NL',
  },
  ...overrides,
});

declare global {
  namespace Cypress {
    interface Chainable {
      // Deletes every contact so each spec starts from a known-empty database
      resetContacts(): Chainable<void>;
      // Creates a contact through the API
      seedContact(overrides?: Partial<ContactPayload>): Chainable<string>;
      // Fills the whole contact form in the UI
      fillContactForm(contact: ContactPayload): Chainable<void>;
      byTestId(id: string): Chainable<JQuery<HTMLElement>>;
    }
  }
}

Cypress.Commands.add('byTestId', (id: string) => cy.get(`[data-testid="${id}"]`));

Cypress.Commands.add('resetContacts', () => {
  const drain = (): void => {
    cy.request('GET', `${API}?page=1&pageSize=100`).then((response) => {
      const items: Array<{ id: string }> = response.body.items ?? [];

      items.forEach((contact) => cy.request('DELETE', `${API}/${contact.id}`));

      if (response.body.totalCount > items.length) {
        drain();
      }
    });
  };

  drain();
});

Cypress.Commands.add('seedContact', (overrides: Partial<ContactPayload> = {}) =>
  cy.request('POST', API, contactFixture(overrides)).then((response) => response.body.id as string),
);

Cypress.Commands.add('fillContactForm', (contact: ContactPayload) => {
  cy.byTestId('firstName').clear().type(contact.firstName);
  cy.byTestId('surname').clear().type(contact.surname);
  cy.get('#dateOfBirth').clear().type(`${contact.dateOfBirth}{esc}`);
  cy.byTestId('phoneNumber').clear().type(contact.phoneNumber);
  cy.byTestId('iban').clear().type(contact.iban);
  cy.byTestId('street').clear().type(contact.address.street);
  cy.byTestId('houseNumber').clear().type(contact.address.houseNumber);
  cy.byTestId('postalCode').clear().type(contact.address.postalCode);
  cy.byTestId('city').clear().type(contact.address.city);
  cy.byTestId('country').clear().type(contact.address.country);
});
