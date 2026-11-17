---
paths:
  - "src/FlipSwitch/**"
  - "src/FlipSwitch.Common/**"
  - "FlipSwitch.Memory/**"
  - "FlipSwitch.SignalR/**"
---

# Client libraries

These projects are what consuming applications reference. Treat their public surface as the product.

## Core abstractions (`src/FlipSwitch/Abstractions`)

- `IConfigurator`: the consumer-facing entry point. `GetConfig<T>(name, ct)`.
- `IBackend`: where raw `Config` entries come from. `HttpServerBackend` is the only implementation.
- `IUpdatable`: implemented by configurators that can accept a pushed `Config` (currently `MemoryCachedConfigurator`).
- `BaseConfigurator`: shared fetch (`GetConfigEntry`) and `DeserializeValue<T>` logic. New configurators should derive from it.

Convenience helpers such as `IsFeatureEnabled<T>` go in `Extensions/ConfiguratorExtensions.cs` as extension methods on `IConfigurator`, not on the interface itself.

## Adding a new client feature

1. Create it as its own project if it brings a dependency (cache provider, transport). The core `FlipSwitch` project should only depend on `Microsoft.Extensions.*` abstractions.
2. Expose it through a `With...()` extension method on `FlipSwitch.DependencyInjection.MicrosoftDependencyInjection.ConfiguratorOptions`, returning the options for chaining.
3. Register through `options.RegisterConfigurator<T>()` and, if it handles pushed updates, `options.RegisterUpdatable<T>()`.

`RegisterConfigurator` and `RegisterUpdatable` each create their own singleton registration, so a type registered through both (as `MemoryCachedConfigurator` is) gets two instances. This works today only because the state lives in the shared `IMemoryCache`. Keep that in mind before adding instance state to a configurator.

All configurators and `ConnectionManager` are singletons. Do not inject scoped services into them.

## Wire contract (`src/FlipSwitch.Common`)

`Config` and `ConfigType` are serialised over both HTTP (`/api/configs`) and SignalR (`Update`). Any change here must be reflected in `FlipSwitch.Web` in the same change: `Api/ConfigsEndpoints.cs` and `Pages/Configs/Edit.cshtml.cs` both build the DTO by hand. Note that neither currently sets `Type`.

## SignalR client (`FlipSwitch.SignalR`)

- `ConnectionManager` builds its URL from `BackendOptions.Url` + `BackendOptions.SignalRPath`, so it requires `.WithOptions()` and a `FlipSwitch:Backend:Url` setting. This is separate from the URL passed to `.WithHttpBackend(...)`.
- It is not started automatically. The host app must start and stop it (see `samples/SimpleWebApplication/ConfigUpdateService.cs`, which is flagged to move out of samples).
