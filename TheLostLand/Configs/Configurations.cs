using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Rocket.Core.Logging;

namespace TheLostLand.Configs;

public sealed class Configurations
{
    private string SaveDir { get; set; }
    private Dictionary<string, Configuration> Configs { get; set; }

    public ModuleConfiguration this[string name] => Configs.ContainsKey(name) ? Configs[name].Config : null;

    internal Configurations(string save_directory, Assembly base_assembly)
    {
        SaveDir = save_directory;
        Configs = new Dictionary<string, Configuration>();

        var all_configs = base_assembly.GetTypes()
            .Where(x => x.BaseType != null)
            .Where(x => x.BaseType == typeof(ModuleConfiguration))
            .Select(x => (x.Name, Activator.CreateInstance(x)));

        foreach (var config in all_configs)
        {
            Load(config.Item2 as ModuleConfiguration);
        }
    }

    internal void Load(ModuleConfiguration config)
    {
        if (Configs.ContainsKey(config.GetType().Name))
            return;
        
        var file_path = Path.Combine(SaveDir, config.GetType().Name, config.GetType().Name + ".json");
        var dir_path = Path.Combine(SaveDir, config.GetType().Name);

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

            Configs.Add(config.GetType().Name, new Configuration(json_data as ModuleConfiguration, config.GetType().Name));
            Logger.Log("Loaded Config: " + config.GetType().Name);
            return;
        }

        Directory.CreateDirectory(dir_path);
        config.LoadDefaults();

        var json_save = JsonConvert.SerializeObject(config, Formatting.Indented);

        using (var stream = new StreamWriter(file_path))
        {
            stream.Write(json_save);
        }

        Configs.Add(config.GetType().Name, new Configuration(config, config.GetType().Name));
    }

    internal void Unload(ModuleConfiguration config)
    {
        if (!Configs.ContainsKey(config.GetType().Name))
        {
            return;
        }

        Configs.Remove(config.GetType().Name);
    }

    internal void Reload(ModuleConfiguration config)
    {
        Unload(config);
        Load(config);
    }

    internal IEnumerable<Configuration> GetAllConfigs(Predicate<Configuration> match) => 
        GetAllConfigs().Where(x => match(x));

    internal IEnumerable<Configuration> GetAllConfigs() => Configs.Select(x => x.Value);
}