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
    public static bool Prefix(InteractableStorage newStorage, PlayerInventory instance)
    {
        var player = UnturnedPlayer.FromPlayer(instance.player);
        var allow = true;
        
        var result = Raycaster.RayCastPlayer(player, RayMasks.BARRICADE_INTERACT);
        if (!result.RaycastHit)
        {
            return true;
        }
        
        OpenStorageEventPublisher.RaiseEvent(player, newStorage, ref allow);
        return allow;
    }
}

[HarmonyPatch(typeof(PlayerInventory))]
[HarmonyPatch("openStorage")]
internal class StorageClosePatch
{
    public static bool Prefix(PlayerInventory instance)
    {
        var player = UnturnedPlayer.FromPlayer(instance.player);
        var currentStorage = instance.storage;
        var allow = true;
        
        var result = Raycaster.RayCastPlayer(player, RayMasks.BARRICADE_INTERACT);
        if (!result.RaycastHit)
        {
            return true;
        }
        
        OpenStorageEventPublisher.RaiseEvent(player, currentStorage, ref allow);
        return allow;
    }
}