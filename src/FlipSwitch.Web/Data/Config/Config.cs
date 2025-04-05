using Marten.Events.Aggregation;

namespace FlipSwitch.Web.Data.Config;

public class Config
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Value { get; set; }
    public DateTimeOffset Created { get; set; }
    public DateTimeOffset LastUpdated { get; set; }
    public string Version { get; set; }

    void Apply(ConfigCreated e)
    {
        Id = e.Id;
        Name = e.Name;
        Value = e.Value;
        Created = e.Created;
        LastUpdated = e.LastUpdated;
        Version = e.Version;
    }

    void Apply(ConfigUpdated e)
    {
        Value = e.Value;
        Version = e.Version;
        LastUpdated = e.LastUpdated;
    }
}