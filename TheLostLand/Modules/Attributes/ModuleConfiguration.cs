namespace TheLostLand.Modules.Attributes;

public class ModuleConfiguration<TConfiguration>
    : ModuleConfiguration where TConfiguration : IConfig
{
    public TConfiguration Configuration => (TConfiguration)Configurations.Instance[ConfigName];

    public ModuleConfiguration(string config_name) : base(config_name) => Configurations.Instance.Load(Activator.CreateInstance<TConfiguration>(), ConfigName);
}

[AttributeUsage(AttributeTargets.Class)]
public class ModuleConfiguration(string config_name) : Attribute
{
    public string ConfigName { get; } = config_name;
}