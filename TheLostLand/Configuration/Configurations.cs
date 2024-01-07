using System.IO;
using Newtonsoft.Json;
using Rocket.Core.Logging;

namespace TheLostLand.Configuration;

public sealed class Configurations : Padlock<Configurations>
{
    private Dictionary<string, Configuration> Configs { get; set; }
    public Configuration this[string config_name] => Configs.ContainsKey(config_name) ? Configs[config_name] : null;

    public Configurations() => Configs = new Dictionary<string, Configuration>();

    internal void Unload(string config_name)
    {
        if (!Configs.ContainsKey(config_name))
        {
            return;
        }

        Configs.Remove(config_name);
    }

    internal void Load(Configuration config)
    {
        if(Configs.ContainsValue(config))
            return;
        
        Configs.Add(config.Name, config);
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