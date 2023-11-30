using Rocket.Core.Plugins;
using TheLostLand.Configs;

namespace TheLostLand;

public class Main : RocketPlugin
{
    public static Main INSTANCE { get; set; }
    public Configurations CONFIGS { get; set; }
    
    protected override void Load()
    {
        INSTANCE = this;
        CONFIGS = new Configurations(Directory, Assembly);
    }

    protected override void Unload()
    {
    }
}