using TheLostLand.Core.Modules.Configuration;

namespace TheLostLand.Modules;

internal class TestingModuleConfig : IConfig
{
    public string TestingProperty { get; set; }
    
    public void LoadDefaults()
    {
        TestingProperty = "Hello";
    }
}