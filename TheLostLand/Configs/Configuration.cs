namespace TheLostLand.Configs;

internal sealed class Configuration
{
    internal ModuleConfiguration Config { get; }
    internal string Name { get; }

    internal Configuration(ModuleConfiguration config, string name)
    {
        Config = config;
        Name = name;
    }
}