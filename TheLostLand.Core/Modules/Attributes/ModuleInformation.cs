using System;

namespace TheLostLand.Core.Modules.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public sealed class ModuleInformation(string module_name) : Attribute
{
    internal string ModuleName { get; } = module_name;
}