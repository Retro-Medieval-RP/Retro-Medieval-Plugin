using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
using TheLostLand.Modules;
using TheLostLand.Modules.Groups;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace TheLostLand.Commands.Groups;

public class CreateGroupCommand : IRocketCommand
{
    public void Execute(IRocketPlayer caller, string[] command)
    {
        if (command.Length < 1)
        {
            UnturnedChat.Say(caller, "Syntax Error: ", Color.red);
            UnturnedChat.Say(caller, Syntax, Color.red);
            return;
        }
        
        if (!ModuleLoader.Instance.GetModule<GroupsModule>(out var groups_module))
        {
            Logger.LogError("Could not find module [GroupsModule]!");
            return;   
        }

        if (groups_module.Exists(command[0]))
        {
            UnturnedChat.Say(caller, "Error: ", Color.red);
            UnturnedChat.Say(caller, "A group with the name " + command[0] + " already exists.", Color.red);
            return;
        }

        if (groups_module.CreateGroup(command[0]))
        {
            UnturnedChat.Say(caller, "A group with the name " + command[0] + " was created.");
            return;
        }
        
        UnturnedChat.Say(caller, "Error: ", Color.red);
        UnturnedChat.Say(caller, "A group with the name " + command[0] + " could not be created.", Color.red);
    }

    public AllowedCaller AllowedCaller => AllowedCaller.Player;
    public string Name => "creategroup";
    public string Help => "Creates a group.";
    public string Syntax => "creategroup <group name>";
    public List<string> Aliases => [];
    public List<string> Permissions => [];
}