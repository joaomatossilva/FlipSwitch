namespace FlipSwitch.Web.Data.Config;

public class ConfigCreated
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Value { get; set; }
    public DateTimeOffset Created { get; set; }
    public DateTimeOffset LastUpdated { get; set; }
    public string Version { get; set; }
}