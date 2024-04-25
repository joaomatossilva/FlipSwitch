namespace FlipSwitch.DependencyInjection;

using Abstractions;
using Microsoft.Extensions.DependencyInjection;

public static class MicrosoftDependencyInjection
{
    public static IServiceCollection AddConfigurator(this IServiceCollection serviceCollection, Action<ConfiguratorOptions> options)
    {
        var configurationOptions = new ConfiguratorOptions(serviceCollection);
        options.Invoke(configurationOptions);
        return serviceCollection;
    }

    public class ConfiguratorOptions(IServiceCollection serviceCollection)
    {
        public ConfiguratorOptions WithHttpBackend(string url)
        {
            serviceCollection.AddHttpClient<IBackend, HttpServerBackend>(client =>
                {
                    client.BaseAddress = new Uri(url);
                });
            return this;
        }

        public ConfiguratorOptions WithNoCache()
        {
            RegisterConfigurator<NoCacheConfigurator>();
            return this;
        }

        // Find a way to hide this, and just allow FlipSwitch libraries
        public void RegisterConfigurator<TConfig>() where TConfig : class, IConfigurator
        {
            serviceCollection.AddScoped<IConfigurator, TConfig>();
        }
    }
}