﻿namespace FlipSwitch;

using Abstractions;
using Common;

public class NoCacheConfigurator(IBackend backend) : IConfigurator
{

    public async Task<bool> IsFeatureEnabled(string name, CancellationToken ct = default)
    {
        var config = await backend.GetConfig(name, ct);

        ct.ThrowIfCancellationRequested();

        if (config.Type != ConfigType.Toggle)
        {
            throw new Exception("Unsupported config type");
        }

        return ConvertToType<bool>(config.Value);
    }

    //TODO: better optimize this code
    //TODO: Extract this to a base type
    protected T ConvertToType<T>(string configValue)
    {
        return (T)Convert.ChangeType(configValue, typeof(T));
    }
}