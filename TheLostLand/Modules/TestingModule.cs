using TheLostLand.Core.Modules;
using TheLostLand.Core.Modules.Attributes;

namespace TheLostLand.Modules;

[ModuleInformation("Testing Module")]
[ModuleConfiguration<TestingModuleConfig>("TestingModuleConfig")]
[ModuleStorage<TestingModuleStorage>("TestingModuleStorage")]
internal class TestingModule : Module
{
    
}