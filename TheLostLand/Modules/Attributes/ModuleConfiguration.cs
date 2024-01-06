namespace TheLostLand.Modules.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class ModuleConfiguration<TConfiguration> : Attribute where TConfiguration : IConfig, new()
{
    private string ConfigName { get; }
    public TConfiguration Configuration => (TConfiguration)Configurations.Instance[ConfigName];
    private string ModuleBase { get; }
    
    public ModuleConfiguration(string config_name, string module_base_name)
    {
        ModuleBase = module_base_name;
        ConfigName = config_name;
        
        Configurations.Instance.Load(new TConfiguration(), ConfigName);
    }

    public bool ModuleMatch(string module_name) => ModuleBase == module_name;
}