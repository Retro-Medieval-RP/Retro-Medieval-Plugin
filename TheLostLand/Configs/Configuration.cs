namespace TheLostLand.Configs.Plugin_Config;

public class Configuration
{
    public IConfig Config { get; }
    public string Name { get; }

    public Configuration(IConfig config, string name)
    {
        Config = config;
        Name = name;
    }
}