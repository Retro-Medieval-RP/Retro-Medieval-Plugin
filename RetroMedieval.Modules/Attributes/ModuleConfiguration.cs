using System;
using System.IO;
using Newtonsoft.Json;
using RetroMedieval.Modules.Configuration;

namespace RetroMedieval.Modules.Attributes;

public sealed class ModuleConfiguration<TConfiguration>(string name) : ModuleConfiguration(name) where TConfiguration : class, IConfig, new()
{
    internal TConfiguration Configuration => GetConfig();
    
    private TConfiguration GetConfig()
    {
        ConfigurationManager.Instance.Get((x => x.ConfigName == Name), out var config);

        return (TConfiguration)config.Config;
    }
    
    internal override bool LoadedConfiguration(string dataPath, string fileName)
    {
        if (ConfigurationManager.Instance.Has(Name))
        {
            return false;
        }

        if (!Directory.Exists(dataPath))
        {
            Directory.CreateDirectory(dataPath);
        }

        var filePath = Path.Combine(dataPath, fileName);

        if (File.Exists(filePath))
        {
            string dataText;
            using (var stream = File.OpenText(filePath))
            {
                dataText = stream.ReadToEnd();
            }

            ConfigurationManager.Instance.Add(
                new Configuration.Configuration(Name, JsonConvert.DeserializeObject<TConfiguration>(dataText)!));
            return true;
        }
        
        {
            var config = new TConfiguration();
            config.LoadDefaults();

            var objData = JsonConvert.SerializeObject(config, Formatting.Indented);

            using var stream = new StreamWriter(filePath, false);
            stream.Write(objData);

            ConfigurationManager.Instance.Add(new Configuration.Configuration(Name, config));
            
            return true;
        }
    }

    internal override bool IsConfigOfType(Type t) => t == typeof(TConfiguration);
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public abstract class ModuleConfiguration(string name) : Attribute
{
    internal string Name { get; } = name;

    internal abstract bool LoadedConfiguration(string dataPath, string fileName);
    internal abstract bool IsConfigOfType(Type t);
}