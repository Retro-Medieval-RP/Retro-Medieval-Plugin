using System.Collections.Generic;
using RetroMedieval.Modules;
using Rocket.API;
using Rocket.Unturned.Chat;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace Kits.Commands.Admin;

internal class DeleteKitCommand : IRocketCommand
{
    public void Execute(IRocketPlayer caller, string[] command)
    {
        if (!ModuleLoader.Instance.GetModule<KitsModule>(out var kitsModule))
        {
            Logger.LogError("Could not find module [KitsModule]!");
            return;   
        }
        
        if (command.Length < 1)
        {
            UnturnedChat.Say(caller, "Syntax Error: ", Color.red);
            UnturnedChat.Say(caller, Syntax, Color.red);
            return;
        }
        
        if (!kitsModule.DoesKitExist(command[0]))
        {
            UnturnedChat.Say(caller, $"A kit with the name ({command[0]}) inputted does not exist!", Color.red);
            return;
        }

        kitsModule.DeleteKit(command[0]);
        UnturnedChat.Say(caller, $"Deleted kit {command[0]} from server.");
    }

    public AllowedCaller AllowedCaller => AllowedCaller.Both; 
    public string Name => "deletekit";
    public string Help => "Deletes a kit from the server.";
    public string Syntax => "deletekit <kit name>";
    public List<string> Aliases => [ "dkit" ];
    public List<string> Permissions => [];
}