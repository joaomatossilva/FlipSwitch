# FlipSwitch
An Enhanced FeatureToggle and possibly more.

The entire idea is to have a rich configuration system that is able to be changed without restarting the application.
Configurations are able to be managed via a centralized admin application.
All values can/should be cached on the client application and signaled when changed.

## :warning: Disclaimer :warning:
This is still in POC phase. Please bear with me!
This contracts can and most certainly will change!

## Client Usage

Register `IConfigurator` using the provided Extensions

```csharp
builder.Services.AddConfigurator(opt => opt
    .WithNoCache()
    .WithHttpBackend("http://localhost:5247"));
```

Inject the `IConfigurator` into the class

```csharp
public class MyClass(IConfigurator configurator)
```

Get the value

```csharp
var enabled = await configurator.IsFeatureEnabled("Teste");
```