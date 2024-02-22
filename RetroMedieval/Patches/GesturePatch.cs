using HarmonyLib;
using RetroMedieval.Events.Unturned;
using Rocket.Unturned.Player;
using SDG.Unturned;

namespace RetroMedieval.Patches;

[HarmonyPatch(typeof(PlayerAnimator))]
[HarmonyPatch("ReceiveGesture")]
internal class GesturePatch
{
    public static bool Prefix(EPlayerGesture newGesture, PlayerAnimator __instance)
    {
        var ply = __instance.player;
        if (ply is null) return false;
        
        var allow = true;
        var player = UnturnedPlayer.FromPlayer(ply);

        GestureEventEventPublisher.RaiseEvent(newGesture, player, ref allow);
        return allow;
    }
}