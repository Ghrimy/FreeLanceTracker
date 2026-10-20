Freelance tracker.

A full-stack client, project, and invoicing tracker for freelancers — built with Blazor and ASP.NET Core Web API.

Show Image

Why I built this

Most portfolio CRUD apps are todo lists. I wanted something with real relational complexity and actual business logic: tracking time against projects, rolling unbilled hours into invoices, and managing invoice state (draft → sent → paid) the way a real freelance billing tool would.

Features

    JWT-based authentication (ASP.NET Core Identity + token issuing/refresh)
    Client management with multiple projects per client
    Time entry logging against projects (billable / non-billable)
    Generate invoices directly from unbilled time entries — the core feature
    Dashboard: active clients and six most recently added or edited projects
    Separate Clients, Projects (including time entries), and Invoices management pages
    Invoice state machine with guard rules (e.g. can't edit a sent invoice)

Tech stack:

    Frontend Blazor (Server)
    Backend ASP.NET Core Web API
    Auth ASP.NET Core Identity + JWT
    Data EF Core + SQL Server

A 3-layer separation (UI → Application → Data) rather than putting logic in code-behind or controllers directly — keeps business rules testable and framework-agnostic.

Project activity tracking

- Apply the `AddProjectActivityTimestamps` migration before using the updated dashboard:
  `dotnet ef database update --project FreeLanceTracker\FreeLanceTracker.csproj`
- New projects and project edits record UTC activity dates. Existing projects keep unknown dates until edited; no historical dates are invented.
- The dashboard excludes archived clients and their projects. Their history remains available on the management pages.


Progress
- Entity model + EF Core relationships (Client, Project, TimeEntry, Invoice, InvoiceLineItem)
- Multi-tenant ownership enforced at the service layer (every query scoped to the logged-in freelancer)
- JWT authentication wired through ASP.NET Core Identity
- Custom exception hierarchy (NotFoundException, InvoiceLockedException, TimeEntryLockedException, ValidationException) + IExceptionHandler pipeline mapping each to the correct HTTP status / ProblemDetails response
- Core feature: generate an invoice from unbilled time entries, grouped by project
- All API endpoints verified working end-to-end via Swagger
- 
