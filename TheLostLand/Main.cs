﻿namespace TheLostLand;

public sealed class Main : RocketPlugin
{
    public static Main Instance { get; private set; }

    protected override void Load()
    {
        Instance = this;
        Configurations.SetInstance(new Configurations(Directory));
    }

    protected override void Unload()
    {
        Configurations.Instance.UnloadAll();
    }
}