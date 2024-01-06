namespace TheLostLand.Modules.Attributes;

public class ModuleConfiguration<TConfiguration>(string config_name) : ModuleConfiguration(config_name)
    where TConfiguration : IConfig, new()
{
    public TConfiguration Configuration =>
        (TConfiguration)Configurations.Instance[ConfigName].Config;

    public override void LoadConfig(string data_path) =>
        Configurations.Instance.Load(new TConfiguration(), ConfigName, data_path);
}

[AttributeUsage(AttributeTargets.Class)]
public abstract class ModuleConfiguration(string config_name) : Attribute
{
    protected string ConfigName { get; } = config_name;

    public abstract void LoadConfig(string data_path);
}