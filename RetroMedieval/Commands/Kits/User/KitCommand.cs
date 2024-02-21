using System.Collections.Generic;
using RetroMedieval.Modules;
using RetroMedieval.Modules.Kits;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using Steamworks;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace RetroMedieval.Commands.Kits.User;

internal class KitCommand : IRocketCommand
{
    public void Execute(IRocketPlayer caller, string[] command)
    {
        if (!ModuleLoader.Instance.GetModule<KitsModule>(out var kits_module))
        {
            Logger.LogError("Could not find module [KitsModule]!");
            return;
        }

        if (command.Length < 1)
        {
            UnturnedChat.Say(caller, "Syntax Error: ", Color.red);
            if (caller.IsAdmin)
            {
                UnturnedChat.Say(caller, AdminSyntax, Color.red);
            }

            UnturnedChat.Say(caller, caller is ConsolePlayer ? AdminSyntax : UserSyntax, Color.red);
            return;
        }

        var target_player = caller as UnturnedPlayer;
        var kit_name = command[0];
        if (command.Length >= 2)
        {
            kit_name = command[1];
            target_player = ulong.TryParse(command[0], out var num)
                ? UnturnedPlayer.FromCSteamID(new CSteamID(num))
                : UnturnedPlayer.FromName(command[0]);

            if (target_player == null)
            {
                UnturnedChat.Say(caller, "Could not find any users specified.", Color.red);
                return;
            }
        }

        if (!kits_module.DoesKitExist(kit_name))
        {
            UnturnedChat.Say(caller, "Could not find kit with name: " + kit_name, Color.red);
            return;
        }

        if (!caller.IsAdmin || caller is not ConsolePlayer || !target_player.HasPermission($"kit.{kit_name}"))
        {
            UnturnedChat.Say(caller,
                $"{(target_player != null && target_player.Equals((UnturnedPlayer)caller) ? "You do " : target_player?.DisplayName + " does ")} not have permission for kit: {kit_name}", Color.red);
            return;
        }

        kits_module.SpawnKit(target_player, kit_name);
        UnturnedChat.Say(caller,
            "Spawned kit " + kit_name +
            $"{(target_player != null && !target_player.Equals((UnturnedPlayer)caller) ? " for " + target_player.DisplayName : "")}");
    }

    public AllowedCaller AllowedCaller => AllowedCaller.Both;
    public string Name => "kit";
    public string Help => "Spawns a kit for a user.";
    public string Syntax => "";
    public List<string> Aliases => [];
    public List<string> Permissions => [];

    private static string AdminSyntax => "kit <player name | player id> <kit name>";
    private static string UserSyntax => "kit <kit name>";
}