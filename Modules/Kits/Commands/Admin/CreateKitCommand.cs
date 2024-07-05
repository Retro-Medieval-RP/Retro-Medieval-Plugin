using System;
using System.Collections.Generic;
using Kits.Models;
using RetroMedieval.Modules;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace Kits.Commands.Admin;

internal class CreateKitCommand : IRocketCommand
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

        var kit = new Kit
        {
            KitName = command[0],
            KitID = Guid.NewGuid()
        };

        if (command.Length >= 2)
        {
            if (int.TryParse(command[1], out var cooldown))
            {
                kit.Cooldown = cooldown;
            }
            else
            {
                UnturnedChat.Say(caller, "Parsing Error: ", Color.red);
                UnturnedChat.Say(caller, "Could not parse cooldown from input.", Color.red);
                UnturnedChat.Say(caller, "Syntax: " + Syntax, Color.red);
                return;
            }
        }

        if (caller is not UnturnedPlayer player)
        {
            return;
        }
        
        var kitItems = new List<KitItem>();
        
        for (byte i = 0; i < PlayerInventory.PAGES; i++)
        {
            if (i == PlayerInventory.AREA)
                continue;

            var count = player.Inventory.getItemCount(i);

            for (byte index = 0; index < count; index++)
            {
                var item = player.Inventory.getItem(i, 0);
                kitItems.Add(new KitItem
                {
                    IsEquipped = i is 0 or 1,
                    Amount = item.item.amount,
                    ItemID = item.item.id,
                    State = item.item.state,
                    Quality = item.item.quality,
                    KitItemID = Guid.NewGuid()
                });
            }
        }
        
        if(player.Player.clothing.backpack != 0)
            kitItems.Add(new KitItem
            {
                IsEquipped = true,
                Amount = 1,
                ItemID = player.Player.clothing.backpack,
                State = player.Player.clothing.backpackState,
                Quality = player.Player.clothing.backpackQuality,
                KitItemID = Guid.NewGuid()
            });
        if(player.Player.clothing.shirt != 0)
            kitItems.Add(new KitItem
            {
                IsEquipped = true,
                Amount = 1,
                ItemID = player.Player.clothing.shirt,
                State = player.Player.clothing.shirtState,
                Quality = player.Player.clothing.shirtQuality,
                KitItemID = Guid.NewGuid()
            });
        if(player.Player.clothing.pants != 0)
            kitItems.Add(new KitItem
            {
                IsEquipped = true,
                Amount = 1,
                ItemID = player.Player.clothing.pants,
                State = player.Player.clothing.pantsState,
                Quality = player.Player.clothing.pantsQuality,
                KitItemID = Guid.NewGuid()
            });
        if(player.Player.clothing.glasses != 0)
            kitItems.Add(new KitItem
            {
                IsEquipped = true,
                Amount = 1,
                ItemID = player.Player.clothing.glasses,
                State = player.Player.clothing.glassesState,
                Quality = player.Player.clothing.glassesQuality,
                KitItemID = Guid.NewGuid()
            });
        if(player.Player.clothing.vest != 0)
            kitItems.Add(new KitItem
            {
                IsEquipped = true,
                Amount = 1,
                ItemID = player.Player.clothing.vest,
                State = player.Player.clothing.vestState,
                Quality = player.Player.clothing.vestQuality,
                KitItemID = Guid.NewGuid()
            });
        if(player.Player.clothing.hat != 0)
            kitItems.Add(new KitItem
            {
                IsEquipped = true,
                Amount = 1,
                ItemID = player.Player.clothing.hat,
                State = player.Player.clothing.hatState,
                Quality = player.Player.clothing.hatQuality,
                KitItemID = Guid.NewGuid()
            });
        
        kitsModule?.CreateKit(kit, kitItems);
        UnturnedChat.Say(caller, $"Created kit: {kit.KitName}");
    }

    public AllowedCaller AllowedCaller => AllowedCaller.Player;
    public string Name => "createkit";
    public string Help => "Creates a kit from the items you are currently wearing.";
    public string Syntax => "createkit <kit name> [cooldown]";
    public List<string> Aliases => ["ckit"];
    public List<string> Permissions => [];
}