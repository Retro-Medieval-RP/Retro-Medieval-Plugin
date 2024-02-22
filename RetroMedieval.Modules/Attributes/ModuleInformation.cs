using System;

namespace RetroMedieval.Modules.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public sealed class ModuleInformation(string module_name) : Attribute
{
    internal string ModuleName { get; } = module_name;
}