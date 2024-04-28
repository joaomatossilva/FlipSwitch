namespace FlipSwitch.SignalR;

using Abstractions;
using Common;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Uri = System.Uri;

public class ConnectionManager
{
    private readonly ILogger<ConnectionManager> logger;
    private readonly IUpdatable updatable;
    private readonly HubConnection connection;

    public ConnectionManager(BackendOptions options, ILogger<ConnectionManager> logger, IUpdatable updatable)
    {
        this.logger = logger;
        this.updatable = updatable;
        connection = new HubConnectionBuilder()
            .WithUrl(new Uri(new Uri(options.Url), options.SignalRPath))
            .Build();

        connection.Closed += async (error) =>
        {
            logger.LogError("Connection was closed. Reconnecting...");
            await Task.Delay(new Random().Next(0, 5) * 1000);
            await connection.StartAsync();
        };

        connection.On("Update", [typeof(Config)], Update);
    }

    public async Task Start(CancellationToken cancellationToken)
    {
        do
        {
            try
            {
                await connection.StartAsync(cancellationToken);
            }
            catch (HttpRequestException exception)
            {
                logger.LogError(exception, "Error Starting");
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                await Task.Delay(500, cancellationToken);
                continue;
            }

            logger.LogInformation("Connected");
            break;
        } while (true);
    }

    private async Task Update(object?[] values)
    {
        if (values.Length == 0 || values[0] == null)
        {
            logger.LogWarning("Received an empty message");
        }

        if (values[0] is Config config)
        {
            await updatable.Update(config);
        }
        else
        {
            logger.LogWarning("Unexpected data from values");
        }
    }

    public async Task Stop(CancellationToken cancellationToken)
    {
        await connection.StopAsync(cancellationToken);
    }
}