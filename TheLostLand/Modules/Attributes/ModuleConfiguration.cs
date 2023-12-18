using TheLostLand.Configuration;

namespace TheLostLand.Modules.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class ModuleConfiguration<TConfiguration> : Attribute where TConfiguration : IConfig, new()
{
    private string ConfigName { get; }
    public TConfiguration Configuration => (TConfiguration)Main.Instance.Configs[ConfigName];
    private string ModuleBase { get; }
    
    public ModuleConfiguration(string config_name, string module_base_name)
    {
        ModuleBase = module_base_name;
        ConfigName = config_name;
        
        Main.Instance.Configs.Load(new TConfiguration(), ConfigName);
    }

    public bool ModuleMatch(string module_name)
    {
        if (ModuleBase == module_name)
        {
            return true;
        }

        return false;
    }
}