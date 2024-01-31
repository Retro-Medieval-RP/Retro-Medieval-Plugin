using System.Collections.Generic;
using System.Linq;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using TheLostLand.Modules;
using TheLostLand.Modules.Groups;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace TheLostLand.Commands.Groups;

public class GroupCommand : IRocketCommand
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
            case "create":
                CreateGroup(caller, command.Skip(1).ToArray());
                break;
            case "delete":
                DeleteGroup(caller, command.Skip(1).ToArray());
                break;
            case "join":
                JoinGroup(caller, command.Skip(1).ToArray());
                break;
            case "leave":
                LeaveGroup(caller, command.Skip(1).ToArray());
                break;
            case "kick":
                KickFromGroup(caller, command.Skip(1).ToArray());
                break;
            case "ban":
                BanFromGroup(caller, command.Skip(1).ToArray());
                break;
            case "unban":
                UnbanFromGroup(caller, command.Skip(1).ToArray());
                break;
        }
    }

    private void UnbanFromGroup(IRocketPlayer caller, string[] command)
    {
    }

    private void BanFromGroup(IRocketPlayer caller, string[] command)
    {
    }

    private void KickFromGroup(IRocketPlayer caller, string[] command)
    {
    }

    private void LeaveGroup(IRocketPlayer caller, string[] command)
    {
    }

    private void JoinGroup(IRocketPlayer caller, string[] command)
    {
    }

    private void DeleteGroup(IRocketPlayer caller, string[] command)
    {
    }

    private static void CreateGroup(IRocketPlayer caller, IReadOnlyList<string> command)
    {
        if (command.Count < 1)
        {
            UnturnedChat.Say(caller, "Syntax Error: ", Color.red);
            UnturnedChat.Say(caller, "group create <group name>", Color.red);
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

        if (groups_module.CreateGroup(command[0], ((UnturnedPlayer)caller).CSteamID.m_SteamID))
        {
            UnturnedChat.Say(caller, "A group with the name " + command[0] + " was created.");
            return;
        }
        
        UnturnedChat.Say(caller, "Error: ", Color.red);
        UnturnedChat.Say(caller, "A group with the name " + command[0] + " could not be created.", Color.red);
    }
    
    public AllowedCaller AllowedCaller => AllowedCaller.Player;
    public string Name => "group";
    public string Help => "General group command";
    public string Syntax => "group <create | delete | join | leave | kick | ban | unban>";
    public List<string> Aliases => 
    [
        "g"
    ];
    public List<string> Permissions => [];
}