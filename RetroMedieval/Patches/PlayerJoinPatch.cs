using HarmonyLib;
using RetroMedieval.Events.Unturned;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;

namespace RetroMedieval.Patches;

[HarmonyPatch(typeof(UnturnedEvents))]
[HarmonyPatch("triggerOnPlayerConnected")]
internal class PlayerJoinPatch
{
    public static bool Prefix(UnturnedPlayer player, UnturnedEvents instance)
    {
        var allow = true;
        PlayerJoinEventEventPublisher.RaiseEvent(player, ref allow);
        
        return allow;
    }
}