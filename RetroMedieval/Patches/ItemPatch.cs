using System;
using HarmonyLib;
using RetroMedieval.Shared.Events.Unturned.Inventory;
using RetroMedieval.Shared.Events.Unturned.Items;
using Rocket.Unturned.Player;
using SDG.Unturned;

namespace RetroMedieval.Patches;

[HarmonyPatch(typeof(PlayerEquipment))]
[HarmonyPatch("ServerEquip")]
public class ServerEquipItemPatch
{
    public static bool Prefix(byte page, byte x, byte y, PlayerEquipment __instance)
    {
        var inv = __instance.player.inventory;

        var allow = true;
        if (page == byte.MaxValue && x == byte.MaxValue && y == byte.MaxValue)
        {
            var equipment = __instance.player.equipment;
            var item = inv.getItem(equipment.equippedPage, inv.getIndex(equipment.equippedPage, equipment.equipped_x, equipment.equipped_y));
            ItemDequipEventPublisher.RaiseEvent(UnturnedPlayer.FromPlayer(__instance.player), item, ref allow);
            return allow;
        }
        
        {
            var item = inv.getItem(page, inv.getIndex(page, x, y));
            ItemEquipEventPublisher.RaiseEvent(UnturnedPlayer.FromPlayer(__instance.player), item, ref allow);
            return allow;
        }
    }
}

[HarmonyPatch(typeof(PlayerEquipment))]
[HarmonyPatch("ReceiveEquip")]
public class ReceiveEquipItemPatch
{
    public static bool Prefix(byte page, byte x, byte y, Guid newAssetGuid, byte newQuality, byte[] newState, NetId useableNetId, PlayerEquipment __instance)
    {
        var inv = __instance.player.inventory;

        var allow = true;
        if (newAssetGuid == Guid.Empty)
        {
            var equipment = __instance.player.equipment;
            var item = inv.getItem(equipment.equippedPage, inv.getIndex(equipment.equippedPage, equipment.equipped_x, equipment.equipped_y));
            ItemDequipEventPublisher.RaiseEvent(UnturnedPlayer.FromPlayer(__instance.player), item, ref allow);
            return allow;
        }

        {

            var item = inv.getItem(page, inv.getIndex(page, x, y));

            ItemEquipEventPublisher.RaiseEvent(UnturnedPlayer.FromPlayer(__instance.player), item, ref allow);
            return allow;
        }
    }
}

[HarmonyPatch(typeof(PlayerEquipment))]
[HarmonyPatch("ReceiveDragItem")]
public class ReceiveDragItemPatch
{
    public static bool Prefix(byte page_0, byte x_0, byte y_0, byte page_1, byte x_1, byte y_1, byte rot_1, PlayerInventory __instance)
    {
        var item = __instance.getItem(page_0, __instance.getIndex(page_0, x_0, y_0));
        var player = UnturnedPlayer.FromPlayer(__instance.player);

        var allow = true;
        ItemDragEventPublisher.RaiseEvent(page_0, x_0, y_0, page_1, x_1, y_1, rot_1, player, item, ref allow);
        return allow;
    }
}

[HarmonyPatch(typeof(PlayerEquipment))]
[HarmonyPatch("ReceiveSwapItem")]
public class ReceiveSwapItemPatch
{
    public static bool Prefix(byte page_0, byte x_0, byte y_0, byte rot_0, byte page_1, byte x_1, byte y_1, byte rot_1, PlayerInventory __instance)
    {
        var item = __instance.getItem(page_0, __instance.getIndex(page_0, x_0, y_0));
        var player = UnturnedPlayer.FromPlayer(__instance.player);
        
        var allow = true;
        ItemSwapEventPublisher.RaiseEvent(page_0, x_0, y_0, rot_0, page_1, x_1, y_1, rot_1, player, item, ref allow);
        return allow;
    }
}

[HarmonyPatch(typeof(PlayerEquipment))]
[HarmonyPatch("ReceiveDropItem")]
public class ReceiveDropItemPatch
{
    public static bool Prefix(byte page, byte x, byte y, PlayerInventory __instance)
    {
        var item = __instance.getItem(page, __instance.getIndex(page, x, y));
        var player = UnturnedPlayer.FromPlayer(__instance.player);
        
        var allow = true;
        ItemDropEventPublisher.RaiseEvent(page, x, y, player, item, ref allow);
        return allow;
    }
}

[HarmonyPatch(typeof(PlayerEquipment))]
[HarmonyPatch("ReceiveUpdateAmount")]
public class ReceiveUpdateAmountItemPatch
{
    public static bool Prefix(byte page, byte index, byte amount, PlayerInventory __instance)
    {
        var item = __instance.getItem(page, index);
        var player = UnturnedPlayer.FromPlayer(__instance.player);
        
        var allow = true;
        ItemUpdateAmountEventPublisher.RaiseEvent(page, index, amount, player, item, ref allow);
        return allow;
    }
}

[HarmonyPatch(typeof(PlayerEquipment))]
[HarmonyPatch("ReceiveUpdateQuality")]
public class ReceiveUpdateQualityItemPatch
{
    public static bool Prefix(byte page, byte index, byte quality, PlayerInventory __instance)
    {
        var item = __instance.getItem(page, index);
        var player = UnturnedPlayer.FromPlayer(__instance.player);
        
        var allow = true;
        ItemUpdateQualityEventPublisher.RaiseEvent(page, index, quality, player, item, ref allow);
        return allow;
    }
}

[HarmonyPatch(typeof(PlayerEquipment))]
[HarmonyPatch("ReceiveUpdateInvState")]
public class ReceiveUpdateInvStateItemPatch
{
    public static bool Prefix(byte page, byte index, byte[] state, PlayerInventory __instance)
    {
        var item = __instance.getItem(page, index);
        var player = UnturnedPlayer.FromPlayer(__instance.player);
        
        var allow = true;
        ItemUpdateInvStateEventPublisher.RaiseEvent(page, index, state, player, item, ref allow);
        return allow;
    }
}

[HarmonyPatch(typeof(PlayerEquipment))]
[HarmonyPatch("ReceiveItemAdd")]
public class ReceiveItemAddItemPatch
{
    public static bool Prefix(byte page, byte x, byte y, byte rot, ushort id, byte amount, byte quality, byte[] state, PlayerInventory __instance)
    {
        var newItem = new ItemJar(x, y, rot, new Item(id, amount, quality, state));
        var player = UnturnedPlayer.FromPlayer(__instance.player);
        
        var allow = true;
        ItemAddEventPublisher.RaiseEvent(page, x, y, rot, player, newItem, ref allow);
        return allow;
    }
}

[HarmonyPatch(typeof(PlayerEquipment))]
[HarmonyPatch("ReceiveItemRemove")]
public class ReceiveItemRemoveItemPatch
{
    public static bool Prefix(byte page, byte x, byte y, PlayerInventory __instance)
    {
        var item = __instance.getItem(page, __instance.getIndex(page, x, y));
        var player = UnturnedPlayer.FromPlayer(__instance.player);
        
        var allow = true;
        ItemRemoveEventPublisher.RaiseEvent(page, x, y, player, item, ref allow);
        return allow;
    }
}

[HarmonyPatch(typeof(PlayerEquipment))]
[HarmonyPatch("ReceiveSize")]
public class ReceiveSizeItemPatch
{
    public static bool Prefix(byte page, byte newWidth, byte newHeight, PlayerInventory __instance)
    {
        var player = UnturnedPlayer.FromPlayer(__instance.player);
        
        var allow = true;
        ClothingSizeEventPublisher.RaiseEvent(page, newWidth, newHeight, player, ref allow);
        return allow;
    }
}