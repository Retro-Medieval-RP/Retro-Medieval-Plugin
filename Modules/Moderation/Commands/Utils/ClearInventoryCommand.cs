using System.Collections.Generic;
using System.Linq;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using UnityEngine;

namespace Moderation.Commands.Utils;

internal class ClearInventoryCommand : IRocketCommand
{
    public void Execute(IRocketPlayer caller, string[] command)
    {
        if (caller is ConsolePlayer && command.Length < 1)
        {
            UnturnedChat.Say(caller, "Syntax Error: ", Color.red);
            UnturnedChat.Say(caller, Syntax, Color.red);
            return;
        }

        UnturnedPlayer target;
        if (command.Length < 1)
        {
            target = caller as UnturnedPlayer;
        }
        else
        {
            target = UnturnedPlayer.FromName(command[0]);
        }
        
        if (target == null)
        {
            if (!ulong.TryParse(command[0], out var targetsID) || Provider.clients.All(x => x.playerID.steamID.m_SteamID != targetsID))
            {
                UnturnedChat.Say(caller, "Target could not be found.", Color.red);
                return;
            }
            
            target = UnturnedPlayer.FromCSteamID(new CSteamID(targetsID));
        }
        
        target.Player.clothing.askWearBackpack(0, 0, [], false);
        target.Player.clothing.askWearGlasses(0, 0, [], false);
        target.Player.clothing.askWearHat(0, 0, [], false);
        target.Player.clothing.askWearPants(0, 0, [], false);
        target.Player.clothing.askWearMask(0, 0, [], false);
        target.Player.clothing.askWearShirt(0, 0, [], false);
        target.Player.clothing.askWearVest(0, 0, [], false);

        for (byte i = 0; i < PlayerInventory.PAGES; i++)
        {
            if (i == PlayerInventory.AREA)
                continue;

            var count = target.Inventory.getItemCount(i);

            for (byte index = 0; index < count; index++)
            {
                target.Inventory.removeItem(i, 0);
            }
        }
        
        UnturnedChat.Say(caller, $"Cleared inventory of user: {target.DisplayName}");
    }

    public AllowedCaller AllowedCaller => AllowedCaller.Both;
    public string Name => "ci";
    public string Help => "Clears a users inventory";
    public string Syntax => "ci [player name | player id]";
    public List<string> Aliases => [];
    public List<string> Permissions => [];
}