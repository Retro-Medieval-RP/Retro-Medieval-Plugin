using System.IO;
using System.Reflection;

namespace TheLostLand.Modules;

public class Module
{
    internal ModuleInformation ModuleInformation { get; set; }

    protected Module()
    {
        GetModuleInfo();
        GetStorages();
        GetConfigs();
    }

    private void GetConfigs()
    {
        var attributes = GetType().GetCustomAttributes<ModuleConfiguration>();

        foreach (var config in attributes)
        {
            config.LoadConfig(Path.Combine(Main.Instance.Directory, ModuleInformation.ModuleName));
        }
    }

    private void GetStorages()
    {
        var attributes = GetType().GetCustomAttributes<ModuleStorage>();

        foreach (var storage in attributes)
        {
            storage.LoadStorage(Path.Combine(Main.Instance.Directory, ModuleInformation.ModuleName));
        }
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

    protected bool GetConfiguration<TConfig>(out TConfig config_out) where TConfig : IConfig, new()
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