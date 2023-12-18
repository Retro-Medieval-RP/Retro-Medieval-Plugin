using TheLostLand.Configs;

namespace TheLostLand.Modules
{
    public abstract class ModuleConfiguration : IConfig
    {
        public abstract void LoadDefaults();
    }
}
