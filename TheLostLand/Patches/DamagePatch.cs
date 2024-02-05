using HarmonyLib;
using SDG.Unturned;
using Steamworks;
using TheLostLand.Events.Unturned;
using UnityEngine;

namespace TheLostLand.Patches;

[HarmonyPatch(typeof(PlayerLife))]
[HarmonyPatch("doDamage")]
internal class DamagePatch
{
    public static bool Prefix(byte amount, Vector3 newRagdoll, EDeathCause newCause, ELimb newLimb, CSteamID newKiller, ref EPlayerKill kill, bool trackKill, ERagdollEffect newRagdollEffect, bool canCauseBleeding, PlayerLife __instance)
    {
        var ply = __instance.channel.owner.player;
        if (ply is null) return false;

        var allow = true;
        DamageEventEventPublisher.RaiseEvent(amount, newRagdoll, newCause, newLimb, newKiller, trackKill, newRagdollEffect, canCauseBleeding, ply, ref kill, ref allow);
        return allow;
    }
}