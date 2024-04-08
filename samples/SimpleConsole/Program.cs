using FlipSwitch;
using FlipSwitch.Abstractions;

var configurator = GetConfigurator();

Console.WriteLine("Press Any Key to Exit");

do
{
    var enabled = await configurator.IsFeatureEnabled("Teste");
    Console.WriteLine($"Teste is {enabled}");
    await Task.Delay(1000);
} while (!Console.KeyAvailable);

return;

// Manual Construct of Configurator. Can be simplified using DI
static IConfigurator GetConfigurator()
{
    var httpClient = new HttpClient();
    httpClient.BaseAddress = new Uri("http://localhost:5247");

    var backend = new HttpServerBackend(httpClient);

    return new NoCacheConfigurator(backend);
}