using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace TheLostLand.Configs;

public class Configurations
{
    private string SAVE_DIR { get; set; }
    private Dictionary<string, IConfig> CONFIGS { get; set; }

    public Configurations(string save_directory, Assembly base_assembly)
    {
        SAVE_DIR = save_directory;
        CONFIGS = new Dictionary<string, IConfig>();

        var all_configs = base_assembly.GetTypes()
            .Where(x => x.BaseType != null)
            .Where(x => x.BaseType == typeof(IConfig) || x.BaseType.GetInterfaces().Contains(typeof(IConfig)))
            .Select(x => (x.Name, Activator.CreateInstance(x)));

        foreach (var config in all_configs)
        {
            if (CONFIGS.ContainsKey(config.Name))
            {
                return;
            }

            Load(config.Item2 as IConfig);
        }
    }

    public void Load(IConfig config)
    {
        var file_path = Path.Combine(SAVE_DIR, config.GetType().Name, config.GetType().Name + ".json");
        var dir_path = Path.Combine(SAVE_DIR, config.GetType().Name);

        if (!Directory.Exists(SAVE_DIR))
        {
            Directory.CreateDirectory(SAVE_DIR);
        }
            
        if (Directory.Exists(dir_path))
        {
            string json;

            using (var stream = File.OpenText(file_path))
            {
                json = stream.ReadToEnd();
            }

            var json_data = JsonConvert.DeserializeObject(json, config.GetType());

            CONFIGS.Add(config.GetType().Name, json_data as IConfig);
            return;
        }

        Directory.CreateDirectory(dir_path);
        config.LoadDefaults();

        var json_save = JsonConvert.SerializeObject(config, Formatting.Indented);

        using (var stream = new StreamWriter(file_path))
        {
            stream.Write(json_save);
        }

        CONFIGS.Add(config.GetType().Name, config);
    }

    public void Unload(IConfig config)
    {
        if (!CONFIGS.ContainsKey(config.GetType().Name))
        {
            return;
        }

        CONFIGS.Remove(config.GetType().Name);
    }

    public void Reload(IConfig config)
    {
        Unload(config);
        Load(config);
    }
}