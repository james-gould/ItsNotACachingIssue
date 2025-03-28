# ItsNotACachingIssue
Proof that you can create and use different DbContexts (with different ContextIds), created in different ways, any update data across services without issue.

[Why does this project exist?](https://www.reddit.com/r/dotnet/comments/1jl1ko3/comment/mjztlj5/)

# Prerequisites

> [!NOTE]
> If you develop `ASP.NET Core` applications using `.NET 8` or above with containers you shouldn't need to configure anything.

- [Please follow this documentation to set up .NET Aspire](https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/setup-tooling?tabs=windows&pivots=visual-studio). 

- If you haven't recently updated Visual Studio (from mid 2024) you may need to install the `Aspire Workload`:

```
dotnet workload install aspire
```

# Run the test

Open `ItsNotACachingIssue.sln` in Visual Studio, then head to the `Test Explorer`. There's 1 test in there, inspect the contents and then run it.

The application flow is:

- `IntegrationTests` initialises the `AppHost` in an `xUnit Integration Testing` environment
- AppHost starts up all services:
    - SqlServer container
        - Database container
    - Migrations WorkerService which applies the `ItsNotACachingIssues/Data/Migrations` to the `UserDbContext.cs`
        - This applies the schema of the `UserDbContext.cs` and seeds a single User with the first name `AAA`.
    - WebApi Project
- `IntegrationTests` gets the endpoint to `UserApi` and connection string to `Database`
- Runs the single test, executing a `PutAsync` to `UserController/ChangeFirstName` changing `User.FirstName` from `AAA` to `BBB`.
- Creates a new instance of the `UserDbContext.cs`, in a different scope + style to the `UserApi`, and sets the connection string.
- Queries out the `User` with `UserId == 1`
- Asserts the `User.FirstName == "BBB"`

# Playground

I've set the `Startup Project` to `AppHost` so you can also F5 and see all of the services scaffolding and then running with their links.

All services have OpenTelemetry enabled by default so any requests through them will have traces etc.