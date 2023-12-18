namespace TheLostLand.Configs;

[System.AttributeUsage(System.AttributeTargets.Class)]
public class ModuleInformation : Attribute
{
    public string ModuleName { get; set; }

    private ModuleConfiguration _ModuleConfig { get; set; }
    public Type ModuleConfig
    {
        get => null;
        set
        {
            if (value.BaseType == null)
                throw new ConfigException($"{value.Name} does not have a base type!");

            if (value.BaseType != typeof(ModuleConfiguration))
                throw new ConfigException($"{value.Name} does not inherit from ModuleConfiguration!");
            
            _ModuleConfig = Activator.CreateInstance(value) as ModuleConfiguration;
        }
    }

    public ModuleConfiguration GetConfig() => _ModuleConfig;
}