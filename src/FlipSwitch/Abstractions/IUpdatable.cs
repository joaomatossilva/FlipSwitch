namespace FlipSwitch.Abstractions;

using Common;

public interface IUpdatable
{
    public Task Update(Config config);
}