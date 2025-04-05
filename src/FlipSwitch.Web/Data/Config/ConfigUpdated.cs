namespace FlipSwitch.Web.Data.Config;

public class ConfigUpdated
{
    public Guid Id { get; set; }
    public string Value { get; set; }
    public DateTimeOffset LastUpdated { get; set; }
    public string Version { get; set; }
}