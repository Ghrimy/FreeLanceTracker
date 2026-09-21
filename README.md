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
    Dashboard: outstanding balance, revenue by client, hours logged this period
    Invoice state machine with guard rules (e.g. can't edit a sent invoice)

Tech stack:

    Frontend Blazor (Server)
    Backend ASP.NET Core Web API
    Auth ASP.NET Core Identity + JWT
    Data EF Core + SQL Server

A 3-layer separation (UI → Application → Data) rather than putting logic in code-behind or controllers directly — keeps business rules testable and framework-agnostic.
