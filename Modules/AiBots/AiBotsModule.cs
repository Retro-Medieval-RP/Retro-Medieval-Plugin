using System;
using System.Collections.Generic;
using System.Linq;
using AiBots.Bot;
using RetroMedieval.Modules;
using RetroMedieval.Modules.Attributes;
using RetroMedieval.Shared.Events.Unturned;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using UnityEngine;

namespace AiBots;

[ModuleInformation("AiBots")]
[ModuleStorage<BotsStorage>("Bots")]
public class AiBotsModule(string directory) : Module(directory)
{
    public List<BotAi> ActiveBots { get; set; } = [];

    public override void Load()
    {
        UnturnedPlayerEvents.OnPlayerDeath += OnPlayerDeath;
        UnturnedPlayerEvents.OnPlayerDead += OnPlayerDead;
        DamageEventPublisher.DamageEventEvent += OnPlayerDamaged;
    }

    public override void Unload()
    {
        UnturnedPlayerEvents.OnPlayerDeath -= OnPlayerDeath;
        UnturnedPlayerEvents.OnPlayerDead -= OnPlayerDead;
        DamageEventPublisher.DamageEventEvent -= OnPlayerDamaged;
    }

    private void OnPlayerDamaged(DamageEventArgs e, ref EPlayerKill kill, ref bool allow)
    {
        var bot = ActiveBots.FirstOrDefault(b => e.Player.channel.owner.playerID.steamID == b.Id);
        if (bot == null)
            return;
        Damage(bot, UnturnedPlayer.FromCSteamID(e.Killer));
    }

    private static async void Damage(BotAi bot, UnturnedPlayer damager) => await bot.Damage(damager, 1);

    private async void OnPlayerDead(UnturnedPlayer player, Vector3 position)
    {
        if (ActiveBots.All(e => e.Id != player.CSteamID))
        {
            return;
        }

        var bot = ActiveBots.FirstOrDefault(e => e.Id == player.CSteamID);
        await bot!.Respawn();
    }

    private void ClearInventory(Player player)
    {
        var playerInv = player.inventory;
        for (byte index1 = 0; index1 < PlayerInventory.PAGES; ++index1)
        {
            if (index1 != PlayerInventory.AREA)
            {
                var itemCount = playerInv.getItemCount(index1);
                for (byte index2 = 0; index2 < itemCount; ++index2)
                    playerInv.removeItem(index1, 0);
            }
        }

        player.clothing.askWearBackpack(0, 0, new byte[0], true);
        Action();
        player.clothing.askWearGlasses(0, 0, new byte[0], true);
        Action();
        player.clothing.askWearHat(0, 0, new byte[0], true);
        Action();
        player.clothing.askWearPants(0, 0, new byte[0], true);
        Action();
        player.clothing.askWearMask(0, 0, new byte[0], true);
        Action();
        player.clothing.askWearShirt(0, 0, new byte[0], true);
        Action();
        player.clothing.askWearVest(0, 0, new byte[0], true);
        Action();
        return;

        void Action()
        {
            for (byte index = 0; index < playerInv.getItemCount(2); ++index)
                playerInv.removeItem(2, 0);
        }
    }

    private void OnPlayerDeath(UnturnedPlayer player, EDeathCause cause, ELimb limb, CSteamID murderer)
    {
        if (ActiveBots.All(e => e.Id != player.CSteamID))
        {
            return;
        }

        ClearInventory(player.Player);
        foreach (var num in ActiveBots.FirstOrDefault(e => e.Id == player.CSteamID)?.Drop!)
        {
            player.GiveItem(num, 1);
        }
    }
}