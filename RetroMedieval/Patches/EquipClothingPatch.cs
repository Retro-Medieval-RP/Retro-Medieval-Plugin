using System;
using HarmonyLib;
using RetroMedieval.Events.Unturned.Clothing;
using Rocket.Unturned.Player;
using SDG.Unturned;

namespace RetroMedieval.Patches;

[HarmonyPatch(typeof(PlayerClothing))]
[HarmonyPatch("ReceiveWearShirt")]
internal class EquipShirtPatch
{
    public static void Prefix(Guid id, byte quality, byte[] state, bool playEffect, PlayerClothing __instance)
    {
        var item = Assets.find(id);
        ShirtEquippedEventPublisher.RaiseEvent(UnturnedPlayer.FromPlayer(__instance.player), item.id);
        ClothingEquipEventPublisher.RaiseEvent(UnturnedPlayer.FromPlayer(__instance.player), item.id);
    }
}

[HarmonyPatch(typeof(PlayerClothing))]
[HarmonyPatch("ReceiveWearPants")]
internal class EquipPantsPatch
{
    public static void Prefix(Guid id, byte quality, byte[] state, bool playEffect, PlayerClothing __instance)
    {
        var item = Assets.find(id);
        PantsEquippedEventPublisher.RaiseEvent(UnturnedPlayer.FromPlayer(__instance.player), item.id);
        ClothingEquipEventPublisher.RaiseEvent(UnturnedPlayer.FromPlayer(__instance.player), item.id);
    }
}

[HarmonyPatch(typeof(PlayerClothing))]
[HarmonyPatch("ReceiveWearHat")]
internal class EquipHatPatch
{
    public static void Prefix(Guid id, byte quality, byte[] state, bool playEffect, PlayerClothing __instance)
    {
        var item = Assets.find(id);
        HatEquippedEventPublisher.RaiseEvent(UnturnedPlayer.FromPlayer(__instance.player), item.id);
        ClothingEquipEventPublisher.RaiseEvent(UnturnedPlayer.FromPlayer(__instance.player), item.id);
    }
}

[HarmonyPatch(typeof(PlayerClothing))]
[HarmonyPatch("ReceiveWearBackpack")]
internal class EquipBackpackPatch
{
    public static void Prefix(Guid id, byte quality, byte[] state, bool playEffect, PlayerClothing __instance)
    {
        var item = Assets.find(id);
        BackpackEquippedEventPublisher.RaiseEvent(UnturnedPlayer.FromPlayer(__instance.player), item.id);
        ClothingEquipEventPublisher.RaiseEvent(UnturnedPlayer.FromPlayer(__instance.player), item.id);
    }
}

[HarmonyPatch(typeof(PlayerClothing))]
[HarmonyPatch("ReceiveWearVest")]
internal class EquipVestPatch
{
    public static void Prefix(Guid id, byte quality, byte[] state, bool playEffect, PlayerClothing __instance)
    {
        var item = Assets.find(id);
        VestEquippedEventPublisher.RaiseEvent(UnturnedPlayer.FromPlayer(__instance.player), item.id);
        ClothingEquipEventPublisher.RaiseEvent(UnturnedPlayer.FromPlayer(__instance.player), item.id);
    }
}

[HarmonyPatch(typeof(PlayerClothing))]
[HarmonyPatch("ReceiveWearMask")]
internal class EquipMaskPatch
{
    public static void Prefix(Guid id, byte quality, byte[] state, bool playEffect, PlayerClothing __instance)
    {
        var item = Assets.find(id);
        MaskEquippedEventPublisher.RaiseEvent(UnturnedPlayer.FromPlayer(__instance.player), item.id);
        ClothingEquipEventPublisher.RaiseEvent(UnturnedPlayer.FromPlayer(__instance.player), item.id);
    }
}

[HarmonyPatch(typeof(PlayerClothing))]
[HarmonyPatch("ReceiveWearGlasses")]
internal class EquipGlassesPatch
{
    public static void Prefix(Guid id, byte quality, byte[] state, bool playEffect, PlayerClothing __instance)
    {
        var item = Assets.find(id);
        GlassesEquippedEventPublisher.RaiseEvent(UnturnedPlayer.FromPlayer(__instance.player), item.id);
        ClothingEquipEventPublisher.RaiseEvent(UnturnedPlayer.FromPlayer(__instance.player), item.id);
    }
}