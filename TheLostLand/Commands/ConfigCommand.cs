using Rocket.Core.Logging;
using Rocket.Unturned.Chat;

namespace TheLostLand.Commands;

public class ConfigCommand : IRocketCommand
{
    public void Execute(IRocketPlayer caller, string[] command)
    {
        if (command.Length < 2)
        {
            UnturnedChat.Say(caller, $"Incorrect Syntax: ");
            UnturnedChat.Say(caller, $"Syntax: {Syntax}");
            return;
        }

        Action<string> config_method;
            
        switch (command[0].ToLower())
        {
            case "reload":
                config_method = ReloadConfig;
                break;
            case "load":
                config_method = LoadConfig;
                break;
            case "unload":
                config_method = UnloadConfig;
                break;
            default:
                return;
        }
        
        switch (command[1].ToLower())
        {
            case "all":
                config_method("all");
                break;
            default:
                var config = command[1];
                config_method(config);
                break;
        }
    }

    private static void UnloadConfig(string obj)
    {
        if (obj == "all")
        {
            Logger.LogError("You cannot unload all configs automatically. Please do this manually.");
            return;
        }

        foreach (var config in Configurations.Instance.GetAllConfigs(x => x.Name == obj))
        {
            Configurations.Instance.Unload(config.Config, config.Name);
        }
    }

    private static void LoadConfig(string obj)
    {
        if (obj == "all")
        {
            Logger.LogError("You cannot unload all configs automatically. Please do this manually.");
            return;
        }
     
        foreach (var config in Configurations.Instance.GetAllConfigs(x => x.Name == obj))
        {
            Configurations.Instance.Load(config.Config, config.Name);
        }   
    }

    private static void ReloadConfig(string obj)
    {
        if (obj == "all")
        {
            foreach (var config in Configurations.Instance.GetAllConfigs())
            {
                Configurations.Instance.Reload(config.Config, config.Name);
            }

            return;
        }

        foreach (var config in Configurations.Instance.GetAllConfigs(x => x.GetType().Name == obj))
        {
            Configurations.Instance.Reload(config.Config, config.Name);
        }
    }


    public AllowedCaller AllowedCaller => AllowedCaller.Both;
    public string Name => "Config";
    public string Help => "This is the command for the config";
    public string Syntax => "Config [reload|load|unload] [all|<Config Name>]";
    public List<string> Aliases => new();
    public List<string> Permissions => new();
}