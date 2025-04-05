namespace FlipSwitch.Abstractions;

using FlipSwitch.Common;
using System.Text.Json;

public abstract class BaseConfigurator(IBackend backend)
{
    protected async Task<Config> GetConfigEntry(string name, CancellationToken ct = default)
    {
        var config = await backend.GetConfig(name, ct);

        ct.ThrowIfCancellationRequested();

        return config;
    }

    protected virtual T DeserializeValue<T>(Config config)
    {
        return JsonSerializer.Deserialize<T>(config.Value)!;
    }
}