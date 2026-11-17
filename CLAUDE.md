# FlipSwitch

An enhanced feature toggle / remote configuration system for .NET. Configs are managed in a central admin app, fetched by client applications over HTTP, cached on the client, and refreshed in real time when they change (no application restart).

**Status: proof of concept.** Contracts can and will change. Prefer simple, direct changes over backwards-compatibility shims.

## Tech stack

- .NET 9 (`net9.0`), C# with nullable reference types and implicit usings enabled
- .NET Aspire 9 for local orchestration (`FlipSwitch.AppHost`, `FlipSwitch.ServiceDefaults`)
- Backend: ASP.NET Core Razor Pages + minimal API + SignalR hub
- Storage: PostgreSQL through Marten (event store with an inline projection)
- Client: plain class libraries, `Microsoft.Extensions.DependencyInjection`, `IMemoryCache`, SignalR client

## Commands

```bash
dotnet build                                  # build the whole solution (FlipSwitch.sln)
dotnet run --project FlipSwitch.AppHost       # run everything through Aspire (needs Docker running)
dotnet run --project src/FlipSwitch.Web       # backend only, http://localhost:5247 (needs a "pg" connection string)
dotnet run --project samples/SimpleConsole    # console sample, expects the backend on http://localhost:5247
dotnet test                                   # runs in CI, but there are no test projects yet
```

The AppHost starts a persistent Postgres container plus pgAdmin, the backend (`backend`) and the sample web app (`web`). `SimpleConsole` is not started by the AppHost.

CI (`.github/workflows/dotnet.yml`) runs `dotnet workload restore`, `restore`, `build` and `test` on pushes and PRs to `main`.

## Solution layout

| Project | Path | Role |
| --- | --- | --- |
| `FlipSwitch` | `src/FlipSwitch` | Client library: `IConfigurator`, `IBackend`, `IUpdatable`, `HttpServerBackend`, `NoCacheConfigurator`, DI builder |
| `FlipSwitch.Common` | `src/FlipSwitch.Common` | Wire contract shared by client and backend: `Config` DTO, `ConfigType` |
| `FlipSwitch.Memory` | `FlipSwitch.Memory` | Client add-on: `MemoryCachedConfigurator` (`IMemoryCache`), `.WithCache()` |
| `FlipSwitch.SignalR` | `FlipSwitch.SignalR` | Client add-on: `ConnectionManager` listens for pushed updates |
| `FlipSwitch.Web` | `src/FlipSwitch.Web` | Backend: admin UI, `GET /api/configs`, `updates` SignalR hub, Marten storage |
| `FlipSwitch.AppHost` | `FlipSwitch.AppHost` | Aspire app host |
| `FlipSwitch.ServiceDefaults` | `FlipSwitch.ServiceDefaults` | Aspire defaults: OpenTelemetry, health checks, service discovery, HTTP resilience |
| `SimpleConsole`, `SimpleWebApplication` | `samples/` | Usage samples |

`FlipSwitch.Memory` and `FlipSwitch.SignalR` live at the repo root on disk even though the solution shows them under the `src` solution folder. Check the real path before adding project references.

## How it fits together

1. An admin creates or edits a config in `FlipSwitch.Web` (`Pages/Configs`). Each change is appended as an event (`ConfigCreated`, `ConfigUpdated`) to a Marten stream keyed by the config's `Guid`. `ConfigProjection` (inline) keeps the `Config` read model current.
2. After an edit, `HubUpdater` broadcasts the new `FlipSwitch.Common.Config` to all SignalR clients as an `Update` message on the `updates` hub.
3. A client app resolves `IConfigurator`. It calls `IBackend.GetConfig(name)`, which `HttpServerBackend` implements as `GET /api/configs?name=...` (returns a list, the first match wins, and it throws if empty).
4. `BaseConfigurator.DeserializeValue<T>` deserialises `Config.Value` (a JSON string) into the caller's type.
5. With `FlipSwitch.Memory`, the entry is cached by name. With `FlipSwitch.SignalR`, `ConnectionManager` receives `Update` and calls `IUpdatable.Update`, which overwrites the cache entry.

Client registration:

```csharp
builder.Services.AddConfigurator(opt => opt
    .WithOptions()                       // binds BackendOptions from "FlipSwitch:Backend" (Url, SignalRPath)
    .WithCache()                         // FlipSwitch.Memory; or .WithNoCache()
    .WithHttpBackend("http://backend")); // Aspire service discovery name, or a real URL

var enabled = await configurator.IsFeatureEnabled<MyConfig>("Teste", c => c.IsActive);
```

## Things that will trip you up

- **Two `Config` classes.** `FlipSwitch.Common.Config` is the wire DTO (string `Id`). `FlipSwitch.Web.Data.Config.Config` is the Marten read model (`Guid Id`, timestamps). Backend code aliases the DTO as `ConfigDto`. The mapping between them is duplicated in `ConfigsEndpoints` and `Edit.cshtml.cs`.
- **Config values are JSON, deserialised with default `System.Text.Json` options**, so property names are case-sensitive. A value for `MyConfig { bool IsActive }` must be `{"IsActive": true}`.
- **`Version` is a fresh GUID string on every create and update**, not a counter.
- **The SignalR message name `Update` and hub path `updates` are hard-coded** on both sides (`HubUpdater`, `Program.cs`, `ConnectionManager`, `BackendOptions.SignalRPath`). Change them together.
- **The connection name `pg` must match** between `FlipSwitch.AppHost/Program.cs` and `AddNpgsqlDataSource("pg")` in the backend.
- **Marten creates its schema automatically** (`AutoCreate.All`). There are no migrations to write or run.
- **SQL Server / EF Core leftovers are dead.** The `flip` connection strings in `appsettings.json`, the commented-out EF packages, `Aspire.Microsoft.Data.SqlClient` in ServiceDefaults, and the `Makefile` `infra` target (it references a `sqlserver` service that `docker-compose.yaml` does not define) predate the move to Postgres and Marten. Use the AppHost for local infrastructure.
- **The README client sample is out of date.** `IsFeatureEnabled` now requires a type argument and a selector (see above).
- **Do not edit `wwwroot/lib/`.** It is vendored Bootstrap and jQuery. The JSON editor on the edit page loads from a CDN in `_Layout.cshtml`.
- The build currently emits about 28 nullable warnings (CS8618 and similar). They are pre-existing; do not add new ones.

## Conventions

- File-scoped namespaces, with `using` directives placed inside the namespace (after the `namespace` line) in library code.
- Primary constructors for dependency injection.
- Async methods take an optional `CancellationToken ct = default` as the last parameter.
- Client features ship as separate packages that extend `ConfiguratorOptions` with `With...()` extension methods (see `FlipSwitch.Memory/DependencyInjection`). Keep the core `FlipSwitch` library free of heavy dependencies.
- Anything that crosses the client/backend boundary belongs in `FlipSwitch.Common`.
- 4-space indentation in C#, no `.editorconfig` yet.
- `TODO` and inline comments such as "should be configurable" mark known gaps the author intends to revisit. Leave them unless the task addresses them.

More detailed, path-scoped guidance lives in `.claude/rules/`.
