namespace FlipSwitch.Memory;

using Abstractions;
using Microsoft.Extensions.Caching.Memory;

public class MemoryCachedConfigurator(IMemoryCache memoryCache, IBackend backend) : BaseConfigurator(backend), IConfigurator
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
}