using System;

namespace TheLostLand.Core.Modules.Attributes;

[System.AttributeUsage(System.AttributeTargets.Class, Inherited = false)]
public class ModuleInformation(string module_name) : Attribute
{
    public string ModuleName { get; set; } = module_name;
}