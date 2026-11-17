---
paths:
  - "src/FlipSwitch.Web/**"
  - "FlipSwitch.AppHost/**"
  - "FlipSwitch.ServiceDefaults/**"
---

# Backend (`FlipSwitch.Web`) and Aspire hosting

## Structure

- `Program.cs`: wiring. Marten, SignalR, Razor Pages, Swagger, `MapConfigsApi()`, `MapHub<UpdatesHub>("updates")`.
- `Api/`: minimal API endpoints, one static class per resource with a `Map...Api(this WebApplication)` extension.
- `Data/Config/`: Marten events (`ConfigCreated`, `ConfigUpdated`), the `Config` read model and `ConfigProjection`.
- `Pages/Configs/`: scaffold-style Razor Pages CRUD for the admin UI.
- `Updater/`: `UpdatesHub` and `HubUpdater` (pushes changes to clients).

## Persistence (Marten event sourcing)

- State changes are events appended to a stream whose id is the config's `Guid`: `session.Events.Append(id, @event)` then `SaveChangesAsync()`. Do not write to the `Config` document directly.
- `ConfigProjection` is registered as `ProjectionLifecycle.Inline`, so the `Config` document is updated in the same transaction and can be queried immediately.
- Adding a field means updating: the event class(es), `ConfigProjection.Apply(...)`, the `Config` read model (it also has its own private `Apply` methods, keep both in step), and the DTO mapping if clients need it.
- Adding a new kind of change means a new event class plus a matching `Apply` in the projection.
- Read with `IQuerySession` (injected) for queries; use `IDocumentStore.LightweightSession()` for writes, as the existing pages do.
- Delete archives the event stream and deletes the projected document.
- Serialisation is System.Text.Json (`UseSystemTextJsonForSerialization`).

## Notifying clients

Any write that changes a config's value should call `HubUpdater.SendConfigUpdate(dto)` after `SaveChangesAsync()`. Today only the Edit page does this; Create and Delete do not notify.

## Admin UI

- Standard Razor Pages with Bootstrap. Page models use view model classes nested in the page model (`CreateViewModel`, `EditViewModel`) rather than binding the read model.
- The Edit page uses JSONEditor (CDN, loaded in `Pages/Shared/_Layout.cshtml`) and copies the editor text into a hidden `Config.Value` field on submit.

## Aspire

- `FlipSwitch.AppHost/Program.cs` defines the resources: `pg` (Postgres, persistent container, pgAdmin), `backend` (`FlipSwitch.Web`), `web` (`SimpleWebApplication`). Resource names are used for service discovery (`http://backend`) and connection strings (`pg`), so renaming one means updating its consumers.
- A new runnable project needs a `ProjectReference` in `FlipSwitch.AppHost.csproj`, an `AddProject<...>` call, and `builder.AddServiceDefaults()` / `app.MapDefaultEndpoints()` in its own `Program.cs`.
- `FlipSwitch.ServiceDefaults/Extensions.cs` is the stock Aspire template. Health endpoints (`/health`, `/alive`) are mapped in Development only.
