using HarmonyLib;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using TheLostLand.Modules;
using TheLostLand.Modules.Death;
using UnityEngine;

namespace TheLostLand.Patches.Death;

[HarmonyPatch(typeof(PlayerLife))]
[HarmonyPatch("doDamage")]
class DamagePatch
{
    public static void Prefix(byte amount, Vector3 newRagdoll, EDeathCause newCause, ELimb newLimb, CSteamID newKiller, ref EPlayerKill kill, bool trackKill, ERagdollEffect newRagdollEffect, bool canCauseBleeding, PlayerLife __instance)
    {
        var ply = __instance.channel.owner.player;
        if (ply is null) return;

        if (amount < ply.life.health)
        {
            return;
        }
        
        if (!ModuleLoader.Instance.GetModule<DeathModule>(out var module))
        {
            return;
        }

        module.SendDeath(UnturnedPlayer.FromPlayer(ply));
    }
}