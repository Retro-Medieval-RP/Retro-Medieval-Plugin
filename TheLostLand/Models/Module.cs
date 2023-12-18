using TheLostLand.Configs;

namespace TheLostLand.Models;

public class Module<TModuleConfiguration> where TModuleConfiguration : class, IConfig
{
    protected TModuleConfiguration Configuration { get; private set; }

    protected Module()
    {
        var attributes = GetType().GetCustomAttributes(typeof(IConfig), false);

        if (attributes.Length < 1)
        {
            return;
        }
        
        Configuration = GetType().GetCustomAttributes(typeof(IConfig), false)[0] as TModuleConfiguration;
    }
}