namespace FlipSwitch.Web.Data;

using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

[Index(nameof(Name), Name = "idx_by_name")]
public class Config
{
    [Key]
    public string Id { get; set; }
    public string Name { get; set; }
    public string Value { get; set; }
    public ConfigType Type { get; set; }
    public DateTimeOffset Created { get; set; }
    public DateTimeOffset LastUpdated { get; set; }
    public string Version { get; set; }
}

public enum ConfigType
{
    Toggle = 0
}