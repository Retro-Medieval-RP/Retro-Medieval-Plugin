using System.Collections.Generic;
using System.Threading.Tasks;
using RetroMedieval.Modules;
using Rocket.API;
using Rocket.Unturned.Chat;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace Kits.Commands.Admin;

internal class RenameKitCommand : IRocketCommand
{
    public async void Execute(IRocketPlayer caller, string[] command)
    {
        if (!ModuleLoader.Instance.GetModule<KitsModule>(out var kitsModule))
        {
            Logger.LogError("Could not find module [KitsModule]!");
            return;   
        }
        
        if (command.Length < 2)
        {
            UnturnedChat.Say(caller, "Syntax Error: ", Color.red);
            UnturnedChat.Say(caller, Syntax, Color.red);
            return;
        }

        if (!await kitsModule.DoesKitExist(command[0]))
        {
            UnturnedChat.Say(caller, $"A kit with the name ({command[0]}) inputted does not exist!", Color.red);
            return;
        }

        kitsModule.RenameKit(command[0], command[1]);
        UnturnedChat.Say(caller, $"Renamed kit from {command[0]} to {command[1]}");
    }

    public AllowedCaller AllowedCaller => AllowedCaller.Both;
    public string Name => "renamekit";
    public string Help => "Renames a kit.";
    public string Syntax => "renamekit <current kit name> <new kit name>";
    public List<string> Aliases => [ "rkit" ];
    public List<string> Permissions => [];
}