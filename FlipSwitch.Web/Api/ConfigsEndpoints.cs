namespace FlipSwitch.Web.Api;

using Data;
using Microsoft.EntityFrameworkCore;
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

    public static async Task<IEnumerable<ConfigDto>> HandleGet(string name, FlipDbContext context)
    {
        var configs = await context.Configs
            .Where(x => x.Name == name)
            .Select(x => new ConfigDto
            {
                Name = x.Name,
                Id = x.Id,
                Type = (Common.ConfigType) x.Type,
                Value = x.Value,
                Version = x.Version
            })
            .ToListAsync();

        return configs;
    }
}