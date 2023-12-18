namespace TheLostLand.Configs;

public class ModuleInformation<TModuleConfiguration> : ModuleInformation where TModuleConfiguration : ModuleConfiguration, new()
{
    public TModuleConfiguration ModuleConfig { get; set; }

    public ModuleInformation(string module_name) : base(module_name) => 
        Main.Instance.Configs.Load(new TModuleConfiguration());
}

[System.AttributeUsage(System.AttributeTargets.Class)]
public class ModuleInformation(string module_name) : Attribute
{
    public string ModuleName { get; set; } = module_name;
}