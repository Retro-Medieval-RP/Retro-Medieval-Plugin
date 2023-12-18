namespace TheLostLand.Configs;

internal sealed class Configuration
{
    internal IConfig Config { get; }
    internal string Name { get; }

    internal Configuration(IConfig config, string name)
    {
        Config = config;
        Name = name;
    }
}