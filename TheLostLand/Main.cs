using TheLostLand.Configs;

namespace TheLostLand;

public sealed class Main : RocketPlugin
{
    public static Main Instance { get; private set; }
    public Configurations Configs { get; set; }
    
    protected override void Load()
    {
        Instance = this;
        Configs = new Configurations(Directory, GetType().Assembly);

        Level.onLevelLoaded += OnLevelLoaded;
    }

    private static void OnLevelLoaded(int level)
    {
    }

    protected override void Unload()
    {
        Level.onLevelLoaded -= OnLevelLoaded;
    }
}