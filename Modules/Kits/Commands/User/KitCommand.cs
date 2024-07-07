using System;
using System.Collections.Generic;
using Kits.Models;
using RetroMedieval.Modules;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using Steamworks;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace Kits.Commands.User;

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

        if (caller is not ConsolePlayer && !targetPlayer.HasPermission($"kit.{kitName}"))
        {
            UnturnedChat.Say(caller,
                $"{(targetPlayer != null && targetPlayer.Equals((UnturnedPlayer)caller) ? "You do" : targetPlayer?.DisplayName + " does")} not have permission for kit: {kitName}", Color.red);
            return;
        }
        
        if(!caller.HasPermission("kit.cooldown.bypass"))
        {
            if (kitsModule.IsKitOnCooldown(targetPlayer, kitName))
            {
                var lastSpawnTime = kitsModule.GetLastSpawnDate(targetPlayer, kitName);
                var kitCooldown = kitsModule.GetCooldown(kitName);
                
                if (kitCooldown == -1)
                {
                    UnturnedChat.Say(caller, "Error has occured when getting kit cooldown.", Color.red);
                    return;
                }
                
                if ((DateTime.Now - lastSpawnTime).TotalSeconds >= kitCooldown)
                {
                    kitsModule.DeleteCooldown(targetPlayer, kitName);
                    kitsModule.SpawnKit(targetPlayer, kitName);
                    UnturnedChat.Say(caller,
                        "Spawned kit " + kitName +
                        $"{(targetPlayer != null && !targetPlayer.Equals((UnturnedPlayer)caller) ? " for " + targetPlayer.DisplayName : "")}");
                    kitsModule.AddCooldown(targetPlayer, kitName);
                    return;
                }
                
                UnturnedChat.Say(caller,
                    $"You have {kitsModule.GetKit(kitName).TimeSpanString(TimeSpan.FromSeconds(kitCooldown - (DateTime.Now - lastSpawnTime).TotalSeconds))} seconds left before you can spawn the kit again.");
                return;
            }

            kitsModule.SpawnKit(targetPlayer, kitName);
            UnturnedChat.Say(caller,
                "Spawned kit " + kitName +
                $"{(targetPlayer != null && !targetPlayer.Equals((UnturnedPlayer)caller) ? " for " + targetPlayer.DisplayName : "")}");
            kitsModule.AddCooldown(targetPlayer, kitName);
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