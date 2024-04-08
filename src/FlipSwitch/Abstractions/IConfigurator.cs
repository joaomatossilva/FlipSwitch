namespace FlipSwitch.Abstractions;

public interface IConfigurator
{
    public Task<bool> IsFeatureEnabled(string name, CancellationToken ct = default);
}