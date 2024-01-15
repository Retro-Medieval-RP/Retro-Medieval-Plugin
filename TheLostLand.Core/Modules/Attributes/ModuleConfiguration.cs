using System;
using System.IO;
using Newtonsoft.Json;
using TheLostLand.Core.Modules.Configuration;

namespace TheLostLand.Core.Modules.Attributes;

public sealed class ModuleConfiguration<TConfiguration>(string name) : ModuleConfiguration(name) where TConfiguration : class, IConfig, new()
{
    internal TConfiguration Configuration => GetConfig();
    
    private TConfiguration GetConfig()
    {
        ConfigurationManager.Instance.Get((x => x.ConfigName == Name), out var config);

        return (TConfiguration)config.Config;
    }
    
    internal override bool LoadedConfiguration(string data_path, string file_name)
    {
        if (ConfigurationManager.Instance.Has(Name))
        {
            return false;
        }

        if (!Directory.Exists(data_path))
        {
            Directory.CreateDirectory(data_path);
        }

        var file_path = Path.Combine(data_path, file_name);

        if (File.Exists(file_path))
        {
            string data_text;
            using (var stream = File.OpenText(file_path))
            {
                data_text = stream.ReadToEnd();
            }

            ConfigurationManager.Instance.Add(
                new Configuration.Configuration(Name, JsonConvert.DeserializeObject<TConfiguration>(data_text)));
            return true;
        }
        
        {
            var config = new TConfiguration();
            config.LoadDefaults();

            var obj_data = JsonConvert.SerializeObject(config, Formatting.Indented);

            using var stream = new StreamWriter(file_path, false);
            stream.Write(obj_data);

            ConfigurationManager.Instance.Add(new Configuration.Configuration(Name, config));
            
            return true;
        }
    }

    internal override bool IsConfigOfType(Type t) => t == typeof(TConfiguration);
}

[AttributeUsage(AttributeTargets.Class)]
public abstract class ModuleConfiguration(string name) : Attribute
{
    internal string Name { get; } = name;

    internal abstract bool LoadedConfiguration(string data_path, string file_name);
    internal abstract bool IsConfigOfType(Type t);
}