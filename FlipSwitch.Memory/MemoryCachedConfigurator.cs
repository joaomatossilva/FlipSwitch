namespace FlipSwitch.Memory;

using Abstractions;
using Common;
using Microsoft.Extensions.Caching.Memory;

public class MemoryCachedConfigurator(IMemoryCache memoryCache, IBackend backend) : BaseConfigurator(backend), IConfigurator, IUpdatable
{
    public async Task<T> GetConfig<T>(string name, CancellationToken ct = default) where T : notnull
    {

        if (memoryCache.TryGetValue(name, out Config? config) && config is not null)
        {
            return DeserializeValue<T>(config);
        }

        config = await base.GetConfigEntry(name, ct);

        //TODO: Caching Options
        memoryCache.Set(name, config);

        return DeserializeValue<T>(config);
    }

    public Task Update(Config config)
    {
        memoryCache.Set(config.Name, config);
        return Task.CompletedTask;
    }
}