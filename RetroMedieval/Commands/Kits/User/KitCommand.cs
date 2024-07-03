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
        if (!ModuleLoader.Instance.GetModule<KitsModule>(out var kitsModule))
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

        var targetPlayer = caller as UnturnedPlayer;
        var kitName = command[0];
        if (command.Length >= 2)
        {
            kitName = command[1];
            targetPlayer = ulong.TryParse(command[0], out var num)
                ? UnturnedPlayer.FromCSteamID(new CSteamID(num))
                : UnturnedPlayer.FromName(command[0]);

            if (targetPlayer == null)
            {
                UnturnedChat.Say(caller, "Could not find any users specified.", Color.red);
                return;
            }
        }

        if (!kitsModule.DoesKitExist(kitName))
        {
            UnturnedChat.Say(caller, "Could not find kit with name: " + kitName, Color.red);
            return;
        }

        if (!caller.IsAdmin || caller is not ConsolePlayer || !targetPlayer.HasPermission($"kit.{kitName}"))
        {
            UnturnedChat.Say(caller,
                $"{(targetPlayer != null && targetPlayer.Equals((UnturnedPlayer)caller) ? "You do " : targetPlayer?.DisplayName + " does ")} not have permission for kit: {kitName}", Color.red);
            return;
        }

        kitsModule.SpawnKit(targetPlayer, kitName);
        UnturnedChat.Say(caller,
            "Spawned kit " + kitName +
            $"{(targetPlayer != null && !targetPlayer.Equals((UnturnedPlayer)caller) ? " for " + targetPlayer.DisplayName : "")}");
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