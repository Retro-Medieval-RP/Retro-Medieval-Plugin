using HarmonyLib;
using RetroMedieval.Shared.Events.Unturned.Storage;
using RetroMedieval.Utils;
using Rocket.Unturned.Player;
using SDG.Unturned;

namespace RetroMedieval.Patches;

[HarmonyPatch(typeof(PlayerInventory))]
[HarmonyPatch("openStorage")]
internal class StorageOpenPatch
{
    public static bool Prefix(InteractableStorage newStorage, PlayerInventory __instance)
    {
        var player = UnturnedPlayer.FromPlayer(__instance.player);
        var allow = true;
        
        var result = Raycaster.RayCastPlayer(player, RayMasks.BARRICADE_INTERACT);
        if (!result.RaycastHit)
        {
            return true;
        }
        
        OpenStorageEventPublisher.RaiseEvent(player, newStorage, result, ref allow);
        return allow;
    }
}

[HarmonyPatch(typeof(PlayerInventory))]
[HarmonyPatch("openStorage")]
internal class StorageClosePatch
{
    public static bool Prefix(PlayerInventory __instance)
    {
        var player = UnturnedPlayer.FromPlayer(__instance.player);
        var currentStorage = __instance.storage;
        var allow = true;
        
        var result = Raycaster.RayCastPlayer(player, RayMasks.BARRICADE_INTERACT);
        if (!result.RaycastHit)
        {
            return true;
        }
        
        OpenStorageEventPublisher.RaiseEvent(player, currentStorage, result, ref allow);
        return allow;
    }
}