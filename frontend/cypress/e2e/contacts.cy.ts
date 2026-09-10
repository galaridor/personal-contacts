import { contactFixture } from '../support/commands';

describe('Personal contacts', () => {
  beforeEach(() => {
    cy.resetContacts();
  });

  it('creates, views, edits and deletes a contact', () => {
    cy.visit('/contacts');
    cy.byTestId('contacts-empty').should('be.visible');

    // --- create ---------------------------------------------------------
    cy.byTestId('add-contact').click();
    cy.location('pathname').should('eq', '/contacts/new');

    cy.fillContactForm(contactFixture());
    cy.byTestId('save-contact').click();

    cy.location('pathname').should('eq', '/contacts');
    cy.byTestId('contact-row').should('have.length', 1).and('contain.text', 'Ada Lovelace');

    // The domain normalizes what was typed: separators are stripped from the phone number and
    // "00" becomes "+", while the IBAN is stored unspaced and re-grouped for display.
    cy.byTestId('contact-row').should('contain.text', '+31201234567');
    cy.byTestId('contact-row').should('contain.text', 'NL91 ABNA 0417 1643 00');

    // --- view -----------------------------------------------------------
    cy.byTestId('view-contact').click();
    cy.byTestId('contact-full-name').invoke('text').invoke('trim').should('equal', 'Ada Lovelace');
    cy.byTestId('detail-phoneNumber').invoke('text').invoke('trim').should('equal', '+31201234567');
    cy.byTestId('detail-iban')
      .invoke('text')
      .invoke('trim')
      .should('equal', 'NL91 ABNA 0417 1643 00');
    cy.byTestId('detail-address').should('contain.text', 'Keizersgracht 123');
    cy.byTestId('back-to-contacts').click();

    // --- edit -----------------------------------------------------------
    cy.byTestId('edit-contact-row').click();
    cy.location('pathname').should('match', /\/contacts\/[0-9a-f-]+\/edit$/);

    cy.byTestId('surname').should('have.value', 'Lovelace').clear().type('Byron');
    cy.byTestId('city').clear().type('Rotterdam');
    cy.byTestId('save-contact').click();

    cy.location('pathname').should('eq', '/contacts');
    cy.byTestId('contact-row')
      .should('have.length', 1)
      .and('contain.text', 'Ada Byron')
      .and('contain.text', 'Rotterdam');

    // --- delete ---------------------------------------------------------
    cy.byTestId('delete-contact').click();
    cy.get('.p-confirmdialog').should('be.visible').contains('button', 'Delete').click();

    cy.byTestId('contacts-empty').should('be.visible');
    cy.byTestId('contact-row').should('not.exist');
  });

  it('blocks an empty form in the browser without calling the API', () => {
    cy.intercept('POST', '/api/contacts').as('create');

    cy.visit('/contacts/new');
    cy.byTestId('save-contact').click();

    cy.byTestId('firstName-error').should('be.visible');
    cy.byTestId('surname-error').should('be.visible');
    cy.location('pathname').should('eq', '/contacts/new');

    cy.get('@create.all').should('have.length', 0);
  });

  it('filters the list by name', () => {
    cy.seedContact({ firstName: 'Ada', surname: 'Lovelace' });
    cy.seedContact({ firstName: 'Grace', surname: 'Hopper', iban: 'DE89 3704 0044 0532 0130 00' });

    cy.visit('/contacts');
    cy.byTestId('contact-row').should('have.length', 2);

    cy.byTestId('search-contacts').type('hop');

    cy.byTestId('contact-row').should('have.length', 1).and('contain.text', 'Grace Hopper');
  });
});
