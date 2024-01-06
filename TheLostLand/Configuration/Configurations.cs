using System.IO;
using Newtonsoft.Json;
using Rocket.Core.Logging;

namespace TheLostLand.Configuration;

public sealed class Configurations : Padlock<Configurations>
{
    private Dictionary<string, Configuration> Configs { get; set; }
    public Configuration this[string config_name] => Configs.ContainsKey(config_name) ? Configs[config_name] : null;

    public Configurations() => Configs = new Dictionary<string, Configuration>();

    internal void Load(IConfig config, string config_name, string path) =>
        Load(new Configuration(config, config_name, Path.Combine(path, config_name + ".json"), path));

    internal void Load(Configuration config)
    {
        if (Configs.ContainsKey(config.Name))
            return;

        if (Directory.Exists(config.ConfigFolderPath))
        {
            string json;

            using (var stream = File.OpenText(config.ConfigFilePath))
            {
                json = stream.ReadToEnd();
            }

            var json_data = JsonConvert.DeserializeObject(json, config.GetType());

            Configs.Add(config.Name, config);
            Logger.Log("Loaded Config: " + config.Name);
            return;
        }

        Directory.CreateDirectory(config.ConfigFolderPath);
        config.Config.LoadDefaults();

        var json_save = JsonConvert.SerializeObject(config, Formatting.Indented);

        using (var stream = File.CreateText(config.ConfigFilePath))
        {
            stream.Write(json_save);
        }

        Configs.Add(config.Name, config);
        Logger.Log("Created & Loaded Config: " + config.Name);
    }

    internal void Unload(string config_name)
    {
        if (!Configs.ContainsKey(config_name))
        {
            return;
        }

        Configs.Remove(config_name);
    }

    internal void Reload(string config_name)
    {
        var conf = Configs[config_name];

        Unload(config_name);
        Load(conf);
    }

    internal IEnumerable<Configuration> GetAllConfigs(Predicate<Configuration> match) =>
        GetAllConfigs().Where(x => match(x));

    internal IEnumerable<Configuration> GetAllConfigs() => Configs.Select(x => x.Value);

    internal void UnloadAll()
    {
        foreach (var config in Configs)
        {
            Unload(config.Value.Name);
        }
    }
}