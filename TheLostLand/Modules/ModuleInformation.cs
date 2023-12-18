using TheLostLand.Configs;

namespace TheLostLand.Modules;

public class ModuleInformation<TModuleConfiguration> : ModuleInformation where TModuleConfiguration : IConfig, new()
{
    public TModuleConfiguration Configuration { get; set; }

    public ModuleInformation(string module_name) : base(module_name)
    {
        Main.Instance.Configs.Load(new TModuleConfiguration());
    }
}

[System.AttributeUsage(System.AttributeTargets.Class, Inherited = false)]
public class ModuleInformation : Attribute
{
    public string ModuleName { get; set; }

    protected ModuleInformation(string module_name)
    {
        ModuleName = module_name;
    }
}