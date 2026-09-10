# contacts-web

The Angular frontend for Personal Contacts. Components come from PrimeNG; layout, spacing and
typography are Tailwind utilities in the templates. See the [repository README](../README.md) for
prerequisites and the design decisions behind the store and validation layers.

```bash
npm install
npm start      # http://localhost:4200, proxies /api to http://localhost:5240
npm run e2e    # Cypress against a running stack
npm run build  # production build
```

The dev server proxies `/api` to the backend (`proxy.conf.json`), so the API has to be running:
`dotnet run --project ../backend/src/Contacts.Api`.

## Layout

```text
src/app/
  core/
    api/              ContactsApiService, the only place that knows the HTTP contract
    formatting/       IBAN grouping pipe, DateOnly <-> Date helpers
    interceptors/     Turns ProblemDetails responses into one ApiError shape
    models/           Wire types mirroring the API contract

  contacts/
    components/       Presentational: contact table, form fields
    form/             The FormGroup definition and its client-side rules
    pages/            Containers: list, form (create + edit), details
    store/            NgRx actions, reducer, selectors, effects

cypress/
  e2e/                The journey spec
  support/            Custom commands and the contact fixture

styles.css            The only stylesheet: Tailwind + PrimeUI imports, layer order, body defaults
```

There are no component stylesheets. Anything visual lives in the template as utility classes, and
colours go through PrimeNG's tokens (`text-muted-color`, `border-surface`) rather than literals, so
they follow the theme.

Components never call `HttpClient`. Pages dispatch actions, effects call the API, and selectors
feed the templates back.
