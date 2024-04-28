namespace FlipSwitch.Web.Updater;

using Common;
using Microsoft.AspNetCore.SignalR;

public class HubUpdater(IHubContext<UpdatesHub> context)
{
    public async Task SendConfigUpdate(Config config)
    {
        await context.Clients.All.SendAsync("Update", config);
    }
}