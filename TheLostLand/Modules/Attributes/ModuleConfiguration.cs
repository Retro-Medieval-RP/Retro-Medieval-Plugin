using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace TheLostLand.Modules.Attributes;

public class ModuleConfiguration<TConfiguration>(string config_name) : ModuleConfiguration(config_name)
    where TConfiguration : class, IConfig, new()
{
    public TConfiguration Configuration => Configurations.Instance[ConfigName].Config as TConfiguration;

    public override void LoadConfig(string data_path)
    {
        var file_path = Path.Combine(data_path, ConfigName + ".json");

        if (!Directory.Exists(data_path))
        {
            Directory.CreateDirectory(data_path);
        }

        if (File.Exists(file_path))
        {
            string json;

            using (var stream = File.OpenText(file_path))
            {
                json = stream.ReadToEnd();
            }

            Configurations.Instance.Load(new Configuration.Configuration(
                JsonConvert.DeserializeObject<TConfiguration>(json), ConfigName, file_path, data_path));
            return;
        }

        var config = Activator.CreateInstance<TConfiguration>();
        config.LoadDefaults();

        var json_save = JsonConvert.SerializeObject(config, Formatting.Indented);

        using (var stream = new StreamWriter(file_path, false))
        {
            stream.Write(json_save);
        }

        Configurations.Instance.Load(new Configuration.Configuration(config, ConfigName, file_path, data_path));
    }
}

[AttributeUsage(AttributeTargets.Class)]
public abstract class ModuleConfiguration(string config_name) : Attribute
{
    protected string ConfigName { get; } = config_name;

    public abstract void LoadConfig(string data_path);
}