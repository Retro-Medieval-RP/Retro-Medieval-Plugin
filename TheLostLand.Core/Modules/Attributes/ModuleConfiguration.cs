﻿using System;
using TheLostLand.Core.Modules.Configuration;

namespace TheLostLand.Core.Modules.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public abstract class ModuleConfiguration(string name) : Attribute
{
    internal IConfig Configuration { get; set; }
    internal string Name { get; } = name;
    
    internal abstract bool LoadedConfiguration(string data_path, string file_name);
    internal abstract bool IsConfigOfType(Type t);
}