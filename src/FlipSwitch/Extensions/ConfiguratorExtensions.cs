using FlipSwitch.Abstractions;

namespace FlipSwitch.Extensions;

public static class ConfiguratorExtensions
{
    public static async Task<bool> IsFeatureEnabled<T>(
        this IConfigurator configurator,
        string name,
        Func<T, bool> selector,
        CancellationToken cancellationToken = default)
        where T : notnull
    {
        var conf = await configurator.GetConfig<T>(name, cancellationToken);
        return selector(conf);
    }
}