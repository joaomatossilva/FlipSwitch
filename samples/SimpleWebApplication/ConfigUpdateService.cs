// Move this out of samples

using FlipSwitch.SignalR;

namespace SimpleWebApplication;

public class ConfigUpdateService(ConnectionManager connectionManager) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await connectionManager.Start(stoppingToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await connectionManager.Stop(cancellationToken);
        await base.StopAsync(cancellationToken);
    }
}