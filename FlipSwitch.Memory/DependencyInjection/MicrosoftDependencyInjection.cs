namespace FlipSwitch.Memory.DependencyInjection;
using ConfiguratorOptions = FlipSwitch.DependencyInjection.MicrosoftDependencyInjection.ConfiguratorOptions;

public static class MicrosoftDependencyInjection
{
    public static ConfiguratorOptions WithCache(this ConfiguratorOptions options)
    {
        options.RegisterConfigurator<MemoryCachedConfigurator>();
        return options;
    }
}