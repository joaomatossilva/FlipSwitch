namespace FlipSwitch;

using Abstractions;

public class NoCacheConfigurator(IBackend backend) : BaseConfigurator(backend), IConfigurator
{
    public async Task<T> GetConfig<T>(string name, CancellationToken ct = default) where T : notnull
    {
        var config = await GetConfigEntry(name, ct);
        return DeserializeValue<T>(config);
    }
}