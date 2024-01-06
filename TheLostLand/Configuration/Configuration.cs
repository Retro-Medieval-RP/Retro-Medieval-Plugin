namespace TheLostLand.Configuration;

public sealed class Configuration
{
    internal IConfig Config { get; }
    internal string Name { get; }
    internal string ConfigFilePath { get; }
    internal string ConfigFolderPath { get; }

    internal Configuration(IConfig config, string name, string file_path, string folder_path)
    {
        Config = config;
        Name = name;

        ConfigFilePath = file_path;
        ConfigFolderPath = folder_path;
    }
}