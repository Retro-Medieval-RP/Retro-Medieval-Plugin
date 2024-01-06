using System.Reflection;

namespace TheLostLand.Modules;

public class Module
{
    private ModuleInformation ModuleInformation { get; }

    protected Module()
    {
        var attributes = GetType().GetCustomAttributes(typeof(ModuleInformation));

        var enumerable = attributes as Attribute[] ?? attributes.ToArray();
        if (enumerable.Length < 1)
        {
            return;
        }

        ModuleInformation = (ModuleInformation)enumerable.ToArray()[0];
    }
    
    protected bool GetConfiguration<TConfig>(out TConfig config_out) where TConfig : IConfig, new()
    {
        var configs = typeof(TConfig).GetCustomAttributes(typeof(ModuleConfiguration<TConfig>), false).Select(x => x as ModuleConfiguration<TConfig>).Where(x => x!.ModuleMatch(ModuleInformation.ModuleName));

        var module_configurations = configs as ModuleConfiguration<TConfig>[] ?? configs.ToArray();
        
        if (!module_configurations.Any())
        {
            config_out = default;
            return false;
        }

        config_out = module_configurations.ToArray()[0].Configuration;
        return true;
    }
}