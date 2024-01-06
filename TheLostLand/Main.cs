namespace TheLostLand;

public sealed class Main : RocketPlugin
{
    public static Main Instance { get; private set; }

    protected override void Load()
    {
        Instance = this;
        Configurations.Instance.SetInstance(new Configurations(Directory));
    }

    protected override void Unload()
    {
    }
}