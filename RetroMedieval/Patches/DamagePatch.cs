﻿using HarmonyLib;
using RetroMedieval.Shared.Events.Unturned;
using SDG.Unturned;
using Steamworks;
using UnityEngine;

namespace RetroMedieval.Patches;

[HarmonyPatch(typeof(PlayerLife))]
[HarmonyPatch("doDamage")]
internal class DamagePatch
{
    public static bool Prefix(byte amount, Vector3 newRagdoll, EDeathCause newCause, ELimb newLimb, CSteamID newKiller, ref EPlayerKill kill, bool trackKill, ERagdollEffect newRagdollEffect, bool canCauseBleeding, PlayerLife __instance)
    {
        try
        {
            var ply = __instance.channel.owner.player;
            if (ply is null) return false;

            var allow = true;
            DamageEventPublisher.RaiseEvent(amount, newRagdoll, newCause, newLimb, newKiller, trackKill,
                newRagdollEffect, canCauseBleeding, ply, ref kill, ref allow);
            return allow;
        }
        catch
        {
            return true;
        }
    }
}