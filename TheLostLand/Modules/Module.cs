using System.Reflection;

namespace TheLostLand.Modules;

public class Module
{
    internal ModuleInformation ModuleInformation { get; set; }

    protected Module()
    {
        GetModuleInfo();
    }

    private void GetModuleInfo()
    {
        var attributes = GetType().GetCustomAttributes(typeof(ModuleInformation));

        var enumerable = attributes as Attribute[] ?? attributes.ToArray();
        if (enumerable.Length < 1)
        {
            return;
        }

        ModuleInformation = (ModuleInformation)enumerable.ToArray()[0];
    }

    protected bool GetConfiguration<TConfig>(out TConfig config_out) where TConfig : IConfig
    {
        if (GetType().GetCustomAttribute(typeof(ModuleConfiguration<TConfig>)) is ModuleConfiguration<TConfig> config)
        {
            config_out = config.Configuration;
            return true;
        }

        config_out = default;
        return false;
    }
}