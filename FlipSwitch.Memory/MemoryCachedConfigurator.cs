namespace FlipSwitch.Memory;

using Abstractions;
using Common;
using Microsoft.Extensions.Caching.Memory;

public class MemoryCachedConfigurator(IMemoryCache memoryCache, IBackend backend) : BaseConfigurator(backend), IConfigurator, IUpdatable
{
    public new async Task<bool> IsFeatureEnabled(string name, CancellationToken ct = default)
    {
        if (memoryCache.TryGetValue(name, out bool isEnabled))
        {
            return isEnabled;
        }

        isEnabled = await base.IsFeatureEnabled(name, ct);

        //TODO: Caching Options
        memoryCache.Set(name, isEnabled);

        return isEnabled;
    }

    public Task Update(Config config)
    {
        var isEnabled = ToBoolean(config);
        memoryCache.Set(config.Name, isEnabled);
        return Task.CompletedTask;
    }
}