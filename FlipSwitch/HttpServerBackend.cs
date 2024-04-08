namespace FlipSwitch;

using System.Net.Http.Json;
using System.Web;
using Abstractions;
using Common;

public class HttpServerBackend(HttpClient client) : IBackend
{
    public async Task<Config> GetConfig(string name, CancellationToken ct = default)
    {
        var configs = await client.GetFromJsonAsync<ICollection<Common.Config>>($"/api/configs?name={HttpUtility.UrlEncode(name)}", ct);
        var config = configs!.FirstOrDefault();
        if (config is null)
        {
            throw new Exception("Config not found");
        }

        return config;
    }
}