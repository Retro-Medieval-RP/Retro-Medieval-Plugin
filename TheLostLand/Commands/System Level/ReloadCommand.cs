using System.Collections.Generic;
using System.Linq;
using Rocket.API;
using Rocket.Unturned.Chat;
using TheLostLand.Modules;
using UnityEngine;

namespace TheLostLand.Commands.System_Level;

internal class ReloadCommand : IRocketCommand
{
    public void Execute(IRocketPlayer caller, string[] command)
    {
        if (command.Length < 1)
        {
            UnturnedChat.Say(caller, "Syntax Error: ", Color.red);
            UnturnedChat.Say(caller, Syntax, Color.red);
            return;
        }

        switch (command[0].ToLower())
        {
            case "config":
                ReloadConfig(caller, command.Skip(1).ToArray());
                break;
            case "storage":
                ReloadStorage(caller, command.Skip(1).ToArray());
                break;
            case "modules":
                ReloadModule(caller, command.Skip(1).ToArray());
                break;
        }
    }

    private void ReloadModule(IRocketPlayer caller, string[] args)
    {
        ModuleLoader.Instance.ReloadAllModules(Main.Instance.Assembly);
    }

    private void ReloadStorage(IRocketPlayer caller, string[] args)
    {
    }

    private void ReloadConfig(IRocketPlayer caller, string[] args)
    {
    }

    public AllowedCaller AllowedCaller => AllowedCaller.Console;
    public string Name => "reload";
    public string Help => "Allows reload of the different components or config or storage.";
    public string Syntax => "reload <config | storage | modules>";
    public List<string> Aliases => [];
    public List<string> Permissions => [];
}