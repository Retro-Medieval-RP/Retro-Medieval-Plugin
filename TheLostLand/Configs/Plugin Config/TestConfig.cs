namespace TheLostLand.Configs.Plugin_Config;

public class TestConfig : IConfig
{
    public string ExampleString { get; set; }
        
    public void LoadDefaults()
    {
        ExampleString = "Example String";
    }
}