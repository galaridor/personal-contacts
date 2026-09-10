# Personal Contacts

A small CRUD app for managing personal contacts. A contact has a first name, surname, date of
birth, address, phone number and IBAN. Everything is stored in PostgreSQL.

Backend is .NET 10 with EF Core, FluentValidation and a CQRS-style split between reads and writes.
Frontend is Angular 21 with NgRx, PrimeNG components and Tailwind CSS for layout.

## Prerequisites

| Tool     | Version  |
| -------- | -------- |
| .NET SDK | 10.0     |
| Node.js  | 22 LTS   |
| Docker   | any      |

## Running it

Start the database:

```bash
docker compose up -d
```

Credentials are in `docker-compose.yml`. Copy `.env.example` to `.env` to override them.

Start the API:

```bash
dotnet run --project backend/src/Contacts.Api
```

It listens on <http://localhost:5240>, with Swagger at `/swagger`. Migrations run on startup, so
there is no separate setup step.

Start the frontend:

```bash
cd frontend
npm install
npm start
```

Open <http://localhost:4200>. The dev server proxies `/api` to the API (see `proxy.conf.json`),
so the browser only ever makes same-origin requests.

## Testing

```bash
dotnet test backend          # domain + application tests
cd frontend && npm run e2e   # Cypress, needs the database, API and dev server running
```

| Suite                        | Tests | Covers                                                    |
| ---------------------------- | ----: | --------------------------------------------------------- |
| `Contacts.Domain.Tests`      |    57 | Entity invariants and value objects                        |
| `Contacts.Application.Tests` |    24 | Command and query handlers, normalisation, validation, paging |
| Cypress                      |     3 | Full journey, search, client-side validation               |

Backend tests use xUnit and Moq.

## Structure

```text
backend/
  src/
    Contacts.Domain          Contact entity, Address/PhoneNumber/Iban value objects
    Contacts.Application     Command and query handlers, validators, DTOs
    Contacts.Infrastructure  EF Core, migrations, repository, read queries
    Contacts.Api             Minimal API endpoints, HTTP contracts, error handling
  tests/
frontend/                    Angular app, with the Cypress suite in cypress/
docker-compose.yml           Local PostgreSQL
```

Dependencies point inwards. `Contacts.Domain` references nothing but the BCL, and
`Contacts.Infrastructure` depends on the domain rather than the other way round.

## API

| Method   | Route                | Success | Failure  |
| -------- | -------------------- | ------- | -------- |
| `GET`    | `/api/contacts`      | 200     | 400      |
| `GET`    | `/api/contacts/{id}` | 200     | 404      |
| `POST`   | `/api/contacts`      | 201     | 400      |
| `PUT`    | `/api/contacts/{id}` | 200     | 400, 404 |
| `DELETE` | `/api/contacts/{id}` | 204     | 404      |

`GET /api/contacts` accepts `?search=` (matches first name or surname), `?page=` (one-based,
default 1) and `?pageSize=` (default 10, max 100). It returns a page, not a bare array:

```json
{ "items": [], "page": 1, "pageSize": 10, "totalCount": 42 }
```

Failures come back as RFC 9457 ProblemDetails.