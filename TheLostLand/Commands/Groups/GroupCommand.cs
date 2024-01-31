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

    private void UnbanFromGroup(IRocketPlayer caller, IReadOnlyList<string> command)
    {
        if (command.Count < 1)
        {
            UnturnedChat.Say(caller, "Syntax Error: ", Color.red);
            UnturnedChat.Say(caller, UnbanFromGroupSyntax, Color.red);
            return;
        }
        
        if (!ModuleLoader.Instance.GetModule<GroupsModule>(out var groups_module))
        {
            Logger.LogError("Could not find module [GroupsModule]!");
            return;   
        }  
        
        if (!groups_module.HasPermission((UnturnedPlayer)caller, PermissionLevel.Unban))
        {
            UnturnedChat.Say(caller, "You do not have permission to unban users from this group!", Color.red);
            return;
        }

        if (groups_module.Unban(command[0]))
        {
            UnturnedChat.Say(caller, "You have unbanned user " + command[0] + " from group.");
            return;
        }

        UnturnedChat.Say(caller, "Could not unban user: " + command[0]);

    }

    private void BanFromGroup(IRocketPlayer caller, IReadOnlyList<string> command)
    {
        if (command.Count < 1)
        {
            UnturnedChat.Say(caller, "Syntax Error: ", Color.red);
            UnturnedChat.Say(caller, BanFromGroupSyntax, Color.red);
            return;
        }
        
        if (!ModuleLoader.Instance.GetModule<GroupsModule>(out var groups_module))
        {
            Logger.LogError("Could not find module [GroupsModule]!");
            return;   
        }
        
        if (!groups_module.HasPermission((UnturnedPlayer)caller, PermissionLevel.Ban))
        {
            UnturnedChat.Say(caller, "You do not have permission to kick users from this group!", Color.red);
            return;
        }

        if (groups_module.Ban(command[0]))
        {
            UnturnedChat.Say(caller, "You have banned user " + command[0] + " from group.");
            return;
        }

        UnturnedChat.Say(caller, "Could not ban user: " + command[0]);

    }

    private void KickFromGroup(IRocketPlayer caller, IReadOnlyList<string> command)
    {
        if (command.Count < 1)
        {
            UnturnedChat.Say(caller, "Syntax Error: ", Color.red);
            UnturnedChat.Say(caller, KickFromGroupSyntax, Color.red);
            return;
        }
        
        if (!ModuleLoader.Instance.GetModule<GroupsModule>(out var groups_module))
        {
            Logger.LogError("Could not find module [GroupsModule]!");
            return;   
        }

        if (!groups_module.HasPermission((UnturnedPlayer)caller, PermissionLevel.Kick))
        {
            UnturnedChat.Say(caller, "You do not have permission to kick users from this group!", Color.red);
            return;
        }

        if (groups_module.Kick(command[0]))
        {
            UnturnedChat.Say(caller, "You have kicked user " + command[0] + " from group.");
            return;
        }

        UnturnedChat.Say(caller, "Could not kick user: " + command[0]);
    }

    private void LeaveGroup(IRocketPlayer caller, IReadOnlyList<string> command)
    {
        if (!ModuleLoader.Instance.GetModule<GroupsModule>(out var groups_module))
        {
            Logger.LogError("Could not find module [GroupsModule]!");
            return;   
        }

        if (groups_module.LeaveGroup((UnturnedPlayer)caller))
        {
            UnturnedChat.Say(caller, "You have left group: " + command[0]);
            return;
        }
        
        UnturnedChat.Say(caller, "Could not leave group: " + command[0]);
    }

    private void JoinGroup(IRocketPlayer caller, IReadOnlyList<string> command)
    {
        if (command.Count < 1)
        {
            UnturnedChat.Say(caller, "Syntax Error: ", Color.red);
            UnturnedChat.Say(caller, JoinGroupSyntax, Color.red);
            return;
        }
        
        if (!ModuleLoader.Instance.GetModule<GroupsModule>(out var groups_module))
        {
            Logger.LogError("Could not find module [GroupsModule]!");
            return;   
        }
        
        if (!groups_module.Exists(command[0]))
        {
            UnturnedChat.Say(caller, "Error: ", Color.red);
            UnturnedChat.Say(caller, "A group with the name " + command[0] + " does not exist.", Color.red);
            return;
        }

        if (groups_module.JoinGroup((UnturnedPlayer)caller, command[0]))
        {
            UnturnedChat.Say(caller, "You have joined group " + command[0]);
            return;
        }
        
        UnturnedChat.Say(caller, "Could not join group: " + command[0]);
    }

    private void DeleteGroup(IRocketPlayer caller, IReadOnlyList<string> command)
    {
        if (command.Count < 1)
        {
            UnturnedChat.Say(caller, "Syntax Error: ", Color.red);
            UnturnedChat.Say(caller, DeleteGroupSyntax, Color.red);
            return;
        }
        
        if (!ModuleLoader.Instance.GetModule<GroupsModule>(out var groups_module))
        {
            Logger.LogError("Could not find module [GroupsModule]!");
            return;   
        }

        if (!groups_module.Exists(command[0]))
        {
            UnturnedChat.Say(caller, "Error: ", Color.red);
            UnturnedChat.Say(caller, "A group with the name " + command[0] + " does not exist.", Color.red);
            return;
        }

        if (!groups_module.AnyMembers(command[0]))
        {
            groups_module.DeleteGroup(command[0]);
            UnturnedChat.Say(caller, "Deleted group with name: " + command[0]);
            return;
        }
        
        UnturnedChat.Say(caller, "The group with name " + command[0] + " still have members.", Color.red);
    }

    private void CreateGroup(IRocketPlayer caller, IReadOnlyList<string> command)
    {
        if (command.Count < 1)
        {
            UnturnedChat.Say(caller, "Syntax Error: ", Color.red);
            UnturnedChat.Say(caller, CreateGroupSyntax, Color.red);
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

    public string CreateGroupSyntax => "group create <group name>";
    public string DeleteGroupSyntax => "group delete <group name>";
    public string JoinGroupSyntax => "group join <group name>";
    public string KickFromGroupSyntax => "group kick <player name | player steam id>";
    public string BanFromGroupSyntax => "group ban <player name | player steam id>";
    public string UnbanFromGroupSyntax => "group unban <player name | player steam id>";
}