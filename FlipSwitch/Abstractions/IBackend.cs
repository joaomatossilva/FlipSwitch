namespace FlipSwitch.Abstractions;

using Common;

public interface IBackend
{
    public Task<Config> GetConfig(string name, CancellationToken ct = default);
}