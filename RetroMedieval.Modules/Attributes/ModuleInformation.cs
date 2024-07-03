using System;

namespace RetroMedieval.Modules.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public sealed class ModuleInformation(string moduleName) : Attribute
{
    internal string ModuleName { get; } = moduleName;
}