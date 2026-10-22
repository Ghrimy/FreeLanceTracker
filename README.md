# FreeLanceTracker

A full-stack freelance management application built with **C#, Blazor, ASP.NET Core, and Entity Framework Core**. FreeLanceTracker helps freelancers manage clients, organize projects, record working hours, and generate invoices from unbilled time entries.

This project is being developed as a portfolio project to demonstrate practical .NET development, relational data modeling, authentication, API design, and business logic.

## Features

### Client management
- Create and manage freelance clients.
- Associate multiple projects with individual clients.
- Keep client-related information organized.

### Project management
- Organize work into projects associated with clients.
- Track time entries against the relevant projects.
- Separate billable and non-billable work.

### Time tracking
- Record working hours against projects.
- Track billable and non-billable time entries.
- Use unbilled time entries as the basis for invoice generation.

### Invoice management
- Generate invoices directly from unbilled time entries.
- Group invoice line items by project.
- Manage invoice statuses: Draft, Sent, and Paid.
- Enforce business rules that restrict modifications to sent invoices.
- Prevent changes to time entries when the associated invoice rules require them to be locked.

### Dashboard
- View outstanding invoice balances.
- Review revenue by client.
- Monitor hours logged during the selected period.

### Authentication and authorization
- ASP.NET Core Identity for user management and authentication.
- JWT bearer authentication for protected API endpoints.
- User-specific data access to help prevent access to other freelancers' records.

### API and error handling
- REST API endpoints for application functionality.
- Swagger/OpenAPI documentation for API exploration during development.
- Custom exception handling for missing resources, validation failures, and locked invoices or time entries.
- Consistent HTTP error responses using Problem Details.

## Technology Stack

| Technology | Purpose |
|---|---|
| C# | Primary programming language |
| .NET 10 | Application framework |
| Blazor Server | Interactive user interface |
| ASP.NET Core Web API | Backend API |
| ASP.NET Core Identity | User authentication and identity management |
| JWT Bearer Authentication | API authentication |
| Entity Framework Core 10 | Object-relational mapping |
| SQL Server | Relational database |
| AutoMapper | Object-to-object mapping |
| Swagger / OpenAPI | API documentation and testing |

## Architecture

The application is organized around separating presentation, application logic, and data access responsibilities.

- **Presentation:** Blazor components provide the interactive user interface.
- **API:** ASP.NET Core controllers expose application functionality over HTTP.
- **Application services:** Client, project, time-entry, and invoice services contain application operations and business rules.
- **Data access:** Entity Framework Core and SQL Server handle persistence and relational data.
- **Authentication:** ASP.NET Core Identity and JWT bearer authentication support authenticated access.

The data model includes the following core entities:

- `ApplicationUser`
- `Client`
- `Project`
- `TimeEntry`
- `Invoice`
- `InvoiceLineItem`

The relationships between these entities support tracking work from clients and projects through to invoice generation.

## Key Engineering Challenges

### Invoice generation from unbilled time

One of the main business features is generating invoices from existing unbilled time entries. This requires selecting eligible entries, associating them with the appropriate projects, grouping them into invoice line items, and maintaining consistent invoice totals.

### Business rule enforcement

Invoice and time-entry operations include restrictions based on invoice state. For example, a sent invoice must not be edited as though it were still a draft.

### User-specific data access

Application services scope data queries to the authenticated freelancer. This is intended to prevent one user from accessing another user's clients, projects, time entries, or invoices.

### Structured error handling

Custom exceptions are handled through ASP.NET Core's exception-handling pipeline, allowing application errors to be translated into appropriate HTTP status codes and Problem Details responses.

## Getting Started

### Prerequisites

Install the following:

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads), or a compatible SQL Server instance
- An IDE such as [JetBrains Rider](https://www.jetbrains.com/rider/) or [Visual Studio](https://visualstudio.microsoft.com/)
- [Git](https://git-scm.com/)

### 1. Clone the repository

```bash
git clone https://github.com/Ghrimy/FreeLanceTracker.git
cd FreeLanceTracker
```

### 2. Configure application settings

Configure the required settings using .NET User Secrets or environment variables. Do not commit credentials, database passwords, or JWT signing keys to source control.

The application expects the following configuration:

| Configuration key | Purpose |
|---|---|
| `ConnectionStrings:DefaultConnection` | SQL Server connection string |
| `Jwt:Key` | Secret used to sign JWTs |
| `Jwt:Issuer` | Expected JWT issuer |
| `Jwt:Audience` | Expected JWT audience |
| `ApiUrl` | Base URL used by the configured HTTP client |

For local development, initialize User Secrets from the project directory:

```bash
dotnet user-secrets init
```

Set the values using your own local configuration:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<your-connection-string>"
dotnet user-secrets set "Jwt:Key" "<your-long-random-signing-key>"
dotnet user-secrets set "Jwt:Issuer" "<your-issuer>"
dotnet user-secrets set "Jwt:Audience" "<your-audience>"
dotnet user-secrets set "ApiUrl" "<your-local-api-base-url>"
```

Replace the example placeholders with values matching your local environment. The JWT issuer and audience must match the values used when generating and validating tokens. Use HTTPS for local API communication where configured.

### 3. Restore dependencies

From the repository root, restore the solution:

```bash
dotnet restore FreeLanceTracker.sln
```

### 4. Apply database migrations

Ensure your SQL Server instance is running and the connection string is correct.

If the project already contains the required EF Core migrations, apply them with:

```bash
dotnet ef database update --project FreeLanceTracker/FreeLanceTracker.csproj
```

If the EF Core CLI is not installed, install it using the official [.NET EF Core tools documentation](https://learn.microsoft.com/en-us/ef/core/cli/dotnet).

If the project does not yet contain an initial migration, create one before applying database updates.

### 5. Run the application

Start the application from the repository root:

```bash
dotnet run --project FreeLanceTracker/FreeLanceTracker.csproj
```

Open the local HTTPS or HTTP address reported by the application.

During development, Swagger is enabled in the development environment. Use the Swagger UI to inspect available API endpoints and test authentication where required.

> **Configuration note:** The exact launch URL, database configuration, and authentication flow depend on your local setup. The API URL and HTTP client registration must be consistent with the way the Blazor frontend calls the API.

## Project Status

FreeLanceTracker is an ongoing portfolio project.

The repository currently documents the core entity model, EF Core relationships, client and project management, time-entry tracking, invoice generation from unbilled time, invoice state restrictions, user-specific data access, JWT authentication, and centralized exception handling.

Further work focuses on improving authentication reliability, expanding automated test coverage, refining the user experience, and strengthening production-readiness.

## Future Improvements

- Add comprehensive unit tests for invoice calculations and business rules.
- Add integration tests for protected API endpoints and authentication.
- Improve token expiration and renewal handling.
- Complete and verify the end-to-end authentication flow between Blazor and the API.
- Add continuous integration for automated builds and tests.
- Improve validation, loading states, and user-facing error messages.
- Document deployment, production configuration, and database setup.
- Review logging, security configuration, and performance.

## Purpose

FreeLanceTracker is designed to demonstrate practical C# and .NET skills through a realistic business application rather than a basic CRUD-only project.

The primary focus is maintainable application architecture, relational data modeling, business rule enforcement, API development, and secure handling of user-specific data.

## License

No license has been specified yet. If you intend to make this project available for reuse, consider adding an appropriate open-source license.
