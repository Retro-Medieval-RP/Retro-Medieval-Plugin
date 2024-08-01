using System.Collections.Generic;
using AiBots.Bot;
using RetroMedieval.Modules;
using RetroMedieval.Modules.Attributes;
using RetroMedieval.Shared.Events.Unturned;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using UnityEngine;

namespace AiBots
{
    [ModuleInformation("AiBots")]
    public class AiBotsModule(string directory) : Module(directory)
    {
        public List<BotAi> ActiveBots { get; set; }
        
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
        }

        private void OnPlayerDead(UnturnedPlayer player, Vector3 position)
        {
        }

        private void OnPlayerDeath(UnturnedPlayer player, EDeathCause cause, ELimb limb, CSteamID murderer)
        {
        }
    }
}