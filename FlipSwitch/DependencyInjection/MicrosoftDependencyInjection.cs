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
            serviceCollection.AddScoped<IConfigurator, NoCacheConfigurator>();
            return this;
        }
    }
}