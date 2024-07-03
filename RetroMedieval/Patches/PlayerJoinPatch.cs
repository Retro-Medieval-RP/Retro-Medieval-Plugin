using HarmonyLib;
using RetroMedieval.Shared.Events;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;

namespace RetroMedieval.Patches;

[HarmonyPatch(typeof(UnturnedEvents))]
[HarmonyPatch("triggerOnPlayerConnected")]
internal class PlayerJoinPatch
{
    public static bool Prefix(UnturnedPlayer player, UnturnedEvents __instance)
    {
        var allow = true;
        PlayerJoinEventEventPublisher.RaiseEvent(player, ref allow);
        
        return allow;
    }
}