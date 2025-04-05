using FlipSwitch.Web.Data.Config;
using Marten;

namespace FlipSwitch.Web.Api;

using ConfigDto = Common.Config;

public static class ConfigsEndpoints
{
    public static WebApplication MapConfigsApi(this WebApplication app)
    {
        app.MapGet("/api/configs", HandleGet)
            .WithName("GetConfigs")
            .WithOpenApi();
        return app;
    }

    public static async Task<IEnumerable<ConfigDto>> HandleGet(string name, IQuerySession session)
    {
        var configs = await session.Query<Config>()
            .Where(x => x.Name == name)
            .Select(x => new ConfigDto
            {
                Name = x.Name,
                Id = x.Id.ToString(),
                Value = x.Value,
                Version = x.Version
            })
            .ToListAsync();

        return configs;
    }
}