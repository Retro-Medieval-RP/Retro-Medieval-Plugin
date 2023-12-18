using System.IO;
using Newtonsoft.Json;
using Rocket.Core.Logging;

namespace TheLostLand.Configuration;

public sealed class Configurations
{
    private string SaveDir { get; set; }
    private Dictionary<string, Configuration> Configs { get; set; }
    public IConfig this[string config_name] => Configs.ContainsKey(config_name) ? Configs[config_name].Config : null;
    
    internal Configurations(string save_directory)
    {
        SaveDir = save_directory;
        Configs = new Dictionary<string, Configuration>();
    }

    internal void Load(IConfig config, string config_name)
    {
        if (Configs.ContainsKey(config_name))
            return;
        
        var file_path = Path.Combine(SaveDir, config_name, config_name + ".json");
        var dir_path = Path.Combine(SaveDir, config_name);

        if (!Directory.Exists(SaveDir))
        {
            Directory.CreateDirectory(SaveDir);
        }
            
        if (Directory.Exists(dir_path))
        {
            string json;

            using (var stream = File.OpenText(file_path))
            {
                json = stream.ReadToEnd();
            }

            var json_data = JsonConvert.DeserializeObject(json, config.GetType());

            Configs.Add(config_name, new Configuration(json_data as IConfig, config_name));
            Logger.Log("Loaded Config: " + config_name);
            return;
        }

        Directory.CreateDirectory(dir_path);
        config.LoadDefaults();

        var json_save = JsonConvert.SerializeObject(config, Formatting.Indented);

        using (var stream = new StreamWriter(file_path))
        {
            stream.Write(json_save);
        }

        Configs.Add(config_name, new Configuration(config, config_name));
    }

    internal void Unload(IConfig config, string config_name)
    {
        if (!Configs.ContainsKey(config_name))
        {
            return;
        }

        Configs.Remove(config_name);
    }

    internal void Reload(IConfig config, string config_name)
    {
        Unload(config, config_name);
        Load(config, config_name);
    }

    internal IEnumerable<Configuration> GetAllConfigs(Predicate<Configuration> match) => 
        GetAllConfigs().Where(x => match(x));

    internal IEnumerable<Configuration> GetAllConfigs() => Configs.Select(x => x.Value);
}