namespace FlipSwitch;

using Abstractions;
using Common;

public class NoCacheConfigurator(IBackend backend) : BaseConfigurator(backend), IConfigurator
{
    public new Task<bool> IsFeatureEnabled(string name, CancellationToken ct = default) => base.IsFeatureEnabled(name, ct);
}