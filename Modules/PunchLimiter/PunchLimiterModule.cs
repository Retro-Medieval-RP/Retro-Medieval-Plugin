using System.Collections.Generic;
using RetroMedieval.Modules;
using RetroMedieval.Modules.Attributes;
using RetroMedieval.Shared.Events.Unturned;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;

namespace PunchLimiter
{
    [ModuleInformation("Punch Limiter")]
    [ModuleConfiguration<LimiterConfig>("PunchLimiterConfiguration")]
    public class PunchLimiterModule(string directory) : Module(directory)
    {
        public override void Load()
        {
            DamageEventPublisher.DamageEventEvent += OnUserDamaged;
        }

        private void OnUserDamaged(DamageEventArgs e, ref EPlayerKill kill, ref bool allow)
        {
            if (e.Killer.m_SteamID == 0)
            {
                allow = true;
                return;
            }

            if (!GetConfiguration<LimiterConfig>(out var config))
            {
                return;
            }

            if (e.Cause != EDeathCause.PUNCH)
            {
                return;
            }
            
            allow = false;
            DamageTool.damage(e.Player, EDeathCause.GUN, ELimb.LEFT_ARM, new CSteamID(0), UnturnedPlayer.FromPlayer(e.Player).Position, config.DamageValue, 1, out _, false);
        }

        public override void Unload()
        {
            DamageEventPublisher.DamageEventEvent -= OnUserDamaged;
        }
    }
}