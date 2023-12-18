using TheLostLand.Configs;

namespace TheLostLand;

public sealed class Main : RocketPlugin
{
    public static Main Instance { get; private set; }
    public Configurations Configs { get; set; }
    
    protected override void Load()
    {
        Instance = this;
        Configs = new Configurations(Directory);
    }

    protected override void Unload()
    {
    }
}