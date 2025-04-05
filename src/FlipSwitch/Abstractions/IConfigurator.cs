namespace FlipSwitch.Abstractions;

public interface IConfigurator
{
    public Task<T> GetConfig<T>(string name, CancellationToken ct = default) where T : notnull;
}