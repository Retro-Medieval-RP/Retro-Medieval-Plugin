﻿namespace TheLostLand.Core.Modules.Configuration;

public sealed class Configuration(string name, IConfig config)
{
    public string ConfigName { get; set; } = name;
    public IConfig Config { get; set; } = config;
}