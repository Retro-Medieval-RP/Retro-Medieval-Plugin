namespace TheLostLand.Configs.Plugin_Config;

internal class TestConfig : IConfig
{
    public string ExampleString { get; set; }
        
    public void LoadDefaults()
    {
        ExampleString = "Example String";
    }
}