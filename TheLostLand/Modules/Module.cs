using System.IO;
using System.Reflection;

namespace TheLostLand.Modules;

public class Module
{
    internal ModuleInformation ModuleInformation { get; set; }
    internal List<ModuleConfiguration> ModuleConfigurations { get; set; }

    internal List<ModuleStorage> ModuleStorages { get; set; }

    protected Module()
    {
        GetModuleInfo();
        GetConfigs();
        GetStorages();
    }

    private void GetConfigs()
    {
        var attributes = GetType().GetCustomAttributes<ModuleConfiguration>();

        foreach (var config in attributes)
        {
            config.LoadConfig(Path.Combine(Main.Instance.Directory, ModuleInformation.ModuleName));
        }

        ModuleConfigurations = attributes.ToList();
    }

    private void GetStorages()
    {
        var attributes = GetType().GetCustomAttributes<ModuleStorage>();

        foreach (var storage in attributes)
        {
            storage.LoadStorage(Path.Combine(Main.Instance.Directory, ModuleInformation.ModuleName));
        }

        ModuleStorages = attributes.ToList();
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

    protected bool GetConfiguration<TConfiguration>(out TConfiguration config) where TConfiguration : class, IConfig, new()
    {
        if (ModuleConfigurations.Exists(x => x.ConfigType(typeof(TConfiguration))))
        {
            config = GetType().GetCustomAttribute<ModuleConfiguration<TConfiguration>>().Configuration;
            return true;
        }

        config = default;
        return false;
    }
}