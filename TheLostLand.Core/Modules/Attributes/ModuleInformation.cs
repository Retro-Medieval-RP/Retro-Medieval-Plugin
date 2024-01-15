using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Rocket.Core.Logging;

namespace TheLostLand.Core.Modules.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public sealed class ModuleInformation(string module_name) : Attribute
{
    internal string ModuleName { get; } = module_name;
}