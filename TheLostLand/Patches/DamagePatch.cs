using HarmonyLib;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using TheLostLand.Events.Unturned;
using TheLostLand.Modules;
using TheLostLand.Modules.Death;
using UnityEngine;

namespace TheLostLand.Patches.Death;

[HarmonyPatch(typeof(PlayerLife))]
[HarmonyPatch("doDamage")]
internal class DamagePatch
{
    public static bool Prefix(byte amount, Vector3 new_ragdoll, EDeathCause new_cause, ELimb new_limb, CSteamID new_killer, ref EPlayerKill kill, bool track_kill, ERagdollEffect new_ragdoll_effect, bool can_cause_bleeding, PlayerLife instance)
    {
        var ply = instance.channel.owner.player;
        if (ply is null) return false;

        var allow = true;
        DamageEventEventPublisher.RaiseEvent(amount, new_ragdoll, new_cause, new_limb, new_killer, track_kill, new_ragdoll_effect, can_cause_bleeding, ply, ref kill, ref allow);
        return allow;
    }
}