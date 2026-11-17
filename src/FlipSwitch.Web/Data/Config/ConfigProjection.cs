using Marten.Events.Aggregation;

namespace FlipSwitch.Web.Data.Config;

public class ConfigProjection : SingleStreamProjection<Config, Guid>
{
    public void Apply(ConfigCreated created, Config config)
    {
        config.Id = created.Id;
        config.Name = created.Name;
        config.Value = created.Value;
        config.Created = created.Created;
        config.LastUpdated = created.LastUpdated;
        config.Version = created.Version;
    }

    public void Apply(ConfigUpdated updated, Config config)
    {
        config.Value = updated.Value;
        config.LastUpdated = updated.LastUpdated;
        config.Version = updated.Version;
    }
}