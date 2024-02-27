using System;
using System.Collections.Generic;
using System.Linq;
using RetroMedieval.Models.Moderation;
using RetroMedieval.Modules;
using RetroMedieval.Modules.Moderation;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace RetroMedieval.Commands.Moderation;

internal class BanCommand : IRocketCommand
{
    public void Execute(IRocketPlayer caller, string[] command)
    {
        if (command.Length < 2)
        {
            UnturnedChat.Say(caller, "Syntax Error: ", Color.red);
            UnturnedChat.Say(caller, Syntax, Color.red);
            return;
        }

        if (!ModuleLoader.Instance.GetModule<ModerationModule>(out var moderation_module))
        {
            Logger.LogError("Could not find module [ModerationModule]!");
            return;
        }

        var ban = new Ban();

        var target_player = UnturnedPlayer.FromName(command[0]);
        if (target_player == null)
        {
            if (ulong.TryParse(command[0], out var ban_target_id))
            {
                ban.TargetID = ban_target_id;
            }
            else
            {
                UnturnedChat.Say(caller, "Target could not be found.", Color.red);
                return;
            }
        }
        else
        {
            ban.TargetID = target_player.CSteamID.m_SteamID;
        }

        ban.PunisherID = caller is ConsolePlayer ? 0 : ulong.Parse(caller.Id);
        ban.Reason = command.ElementAtOrDefault(1);
        var length = ModerationAction.ConvertToBanDuration(command.Skip(2));
        ban.BanLength = length ?? -1;
        ban.PunishmentGiven = DateTime.Now;
        
        moderation_module.Ban(ban, Provider.clients.Any(x => x.playerID.steamID.m_SteamID == ban.TargetID));
    }

    public AllowedCaller AllowedCaller => AllowedCaller.Both;
    public string Name => "ban";
    public string Help => "Bans a user from a server.";
    public string Syntax => "ban <player name | player id> <reason> [time span]";
    public List<string> Aliases => [];
    public List<string> Permissions => [];
}