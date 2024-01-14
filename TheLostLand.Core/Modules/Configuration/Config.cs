namespace TheLostLand.Core.Modules.Configuration;

public sealed class Config(string name, IConfig config)
{
    public string ConfigName { get; set; } = name;
    public IConfig Configuration { get; set; } = config;
}