using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TheLostLand.Core.Modules.Attributes;
using TheLostLand.Core.Modules.Configuration;
using TheLostLand.Core.Modules.Storage;

namespace TheLostLand.Core.Modules;

public class Module
{
    internal IEnumerable<ModuleConfiguration> Configurations { get; set; }
    internal IEnumerable<ModuleStorage> Storages { get; set; }
    
    protected bool GetConfiguration<TConfiguration>(out TConfiguration config) where TConfiguration : class, IConfig, new()
    {
        if (Configurations.Any(x => x.IsConfigOfType(typeof(TConfiguration))))
        {
            config = GetType().GetCustomAttribute<ModuleConfiguration<TConfiguration>>().Configuration;
            return true;
        }

        config = default;
        return false;
    }

    protected bool GetStorage<TStorage>(out TStorage storage) where TStorage : class, IStorage, new()
    {
        if (Storages.Any(x => x.IsStorageOfType(typeof(TStorage))))
        {
            storage = GetType().GetCustomAttribute<ModuleStorage<TStorage>>().Storage;
            return true;
        }

        storage = default;
        return false;
    }
}