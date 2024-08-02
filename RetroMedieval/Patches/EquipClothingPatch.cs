using System;
using HarmonyLib;
using RetroMedieval.Shared.Events.Unturned.CloathingDequip;
using RetroMedieval.Shared.Events.Unturned.ClothingEquip;
using Rocket.Unturned.Player;
using SDG.Unturned;

namespace RetroMedieval.Patches;

[HarmonyPatch(typeof(PlayerClothing))]
[HarmonyPatch("ReceiveWearShirt")]
internal class EquipShirtPatch
{
    public static bool Prefix(Guid id, byte quality, byte[] state, bool playEffect, PlayerClothing __instance)
    {
        try
        {
            var allow = true;
            if (__instance.player == null)
            {
                return allow;
            }

            if (id == Guid.Empty)
            {
                ShirtDequippedEventPublisher.RaiseEvent(UnturnedPlayer.FromPlayer(__instance.player), __instance.player.clothing.shirt, ref allow);
                ClothingDequipEventPublisher.RaiseEvent(UnturnedPlayer.FromPlayer(__instance.player), __instance.player.clothing.shirt, ref allow);
                return allow;
            }

            var item = Assets.find(id);
            ShirtEquippedEventPublisher.RaiseEvent(UnturnedPlayer.FromPlayer(__instance.player), item.id, ref allow);
            ClothingEquipEventPublisher.RaiseEvent(UnturnedPlayer.FromPlayer(__instance.player), item.id, ref allow);
            return allow;
        }
        catch
        {
            return true;
        }
    }
}

[HarmonyPatch(typeof(PlayerClothing))]
[HarmonyPatch("ReceiveWearPants")]
internal class EquipPantsPatch
{
    public static bool Prefix(Guid id, byte quality, byte[] state, bool playEffect, PlayerClothing __instance)
    {
        try
        {
            var allow = true;
            if (__instance.player == null)
            {
                return true;
            }

            if (id == Guid.Empty)
            {
                PantsDequippedEventPublisher.RaiseEvent(UnturnedPlayer.FromPlayer(__instance.player), __instance.player.clothing.pants, ref allow);
                ClothingDequipEventPublisher.RaiseEvent(UnturnedPlayer.FromPlayer(__instance.player), __instance.player.clothing.pants, ref allow);
                return allow;
            }

            var item = Assets.find(id);
            PantsEquippedEventPublisher.RaiseEvent(UnturnedPlayer.FromPlayer(__instance.player), item.id, ref allow);
            ClothingEquipEventPublisher.RaiseEvent(UnturnedPlayer.FromPlayer(__instance.player), item.id, ref allow);
            return allow;
        }
        catch
        {
            return true;
        }
    }
}

[HarmonyPatch(typeof(PlayerClothing))]
[HarmonyPatch("ReceiveWearHat")]
internal class EquipHatPatch
{
    public static bool Prefix(Guid id, byte quality, byte[] state, bool playEffect, PlayerClothing __instance)
    {
        try
        {
            var allow = true;
            if (__instance.player == null)
            {
                return true;
            }

            if (id == Guid.Empty)
            {
                HatDequippedEventPublisher.RaiseEvent(UnturnedPlayer.FromPlayer(__instance.player), __instance.player.clothing.hat, ref allow);
                ClothingDequipEventPublisher.RaiseEvent(UnturnedPlayer.FromPlayer(__instance.player), __instance.player.clothing.hat, ref allow);
                return allow;
            }

            var item = Assets.find(id);
            HatEquippedEventPublisher.RaiseEvent(UnturnedPlayer.FromPlayer(__instance.player), item.id, ref allow);
            ClothingEquipEventPublisher.RaiseEvent(UnturnedPlayer.FromPlayer(__instance.player), item.id, ref allow);
            return allow;
        }
        catch
        {
            return true;
        }
    }
}

[HarmonyPatch(typeof(PlayerClothing))]
[HarmonyPatch("ReceiveWearBackpack")]
internal class EquipBackpackPatch
{
    public static bool Prefix(Guid id, byte quality, byte[] state, bool playEffect, PlayerClothing __instance)
    {
        try
        {
            var allow = true;
            if (__instance.player == null)
            {
                return true;
            }

            if (id == Guid.Empty)
            {
                BackpackDequippedEventPublisher.RaiseEvent(UnturnedPlayer.FromPlayer(__instance.player), __instance.player.clothing.backpack, ref allow);
                ClothingDequipEventPublisher.RaiseEvent(UnturnedPlayer.FromPlayer(__instance.player), __instance.player.clothing.backpack, ref allow);
                return allow;
            }

            var item = Assets.find(id);
            BackpackEquippedEventPublisher.RaiseEvent(UnturnedPlayer.FromPlayer(__instance.player), item.id, ref allow);
            ClothingEquipEventPublisher.RaiseEvent(UnturnedPlayer.FromPlayer(__instance.player), item.id, ref allow);
            return allow;
        }
        catch
        {
            return true;
        }
    }
}

[HarmonyPatch(typeof(PlayerClothing))]
[HarmonyPatch("ReceiveWearVest")]
internal class EquipVestPatch
{
    public static bool Prefix(Guid id, byte quality, byte[] state, bool playEffect, PlayerClothing __instance)
    {
        try
        {
            var allow = true;
            if (__instance.player == null)
            {
                return true;
            }

            if (id == Guid.Empty)
            {
                VestDequippedEventPublisher.RaiseEvent(UnturnedPlayer.FromPlayer(__instance.player), __instance.player.clothing.vest, ref allow);
                ClothingDequipEventPublisher.RaiseEvent(UnturnedPlayer.FromPlayer(__instance.player), __instance.player.clothing.vest, ref allow);
                return allow;
            }

            var item = Assets.find(id);
            VestEquippedEventPublisher.RaiseEvent(UnturnedPlayer.FromPlayer(__instance.player), item.id, ref allow);
            ClothingEquipEventPublisher.RaiseEvent(UnturnedPlayer.FromPlayer(__instance.player), item.id, ref allow);
            return allow;
        }
        catch
        {
            return true;
        }
    }
}

[HarmonyPatch(typeof(PlayerClothing))]
[HarmonyPatch("ReceiveWearMask")]
internal class EquipMaskPatch
{
    public static bool Prefix(Guid id, byte quality, byte[] state, bool playEffect, PlayerClothing __instance)
    {
        try
        {
            var allow = true;
            if (__instance.player == null)
            {
                return true;
            }

            if (id == Guid.Empty)
            {
                MaskDequippedEventPublisher.RaiseEvent(UnturnedPlayer.FromPlayer(__instance.player), __instance.player.clothing.mask, ref allow);
                ClothingDequipEventPublisher.RaiseEvent(UnturnedPlayer.FromPlayer(__instance.player), __instance.player.clothing.mask, ref allow);
                return allow;
            }

            var item = Assets.find(id);
            MaskEquippedEventPublisher.RaiseEvent(UnturnedPlayer.FromPlayer(__instance.player), item.id, ref allow);
            ClothingEquipEventPublisher.RaiseEvent(UnturnedPlayer.FromPlayer(__instance.player), item.id, ref allow);
            return allow;
        }
        catch
        {
            return true;
        }
    }
}

[HarmonyPatch(typeof(PlayerClothing))]
[HarmonyPatch("ReceiveWearGlasses")]
internal class EquipGlassesPatch
{
    public static bool Prefix(Guid id, byte quality, byte[] state, bool playEffect, PlayerClothing __instance)
    {
        try
        {
            var allow = true;
            if (__instance.player == null)
            {
                return true;
            }

            if (id == Guid.Empty)
            {
                GlassesDequippedEventPublisher.RaiseEvent(UnturnedPlayer.FromPlayer(__instance.player), __instance.player.clothing.glasses, ref allow);
                ClothingDequipEventPublisher.RaiseEvent(UnturnedPlayer.FromPlayer(__instance.player), __instance.player.clothing.glasses, ref allow);
                return allow;
            }

            var item = Assets.find(id);
            GlassesEquippedEventPublisher.RaiseEvent(UnturnedPlayer.FromPlayer(__instance.player), item.id, ref allow);
            ClothingEquipEventPublisher.RaiseEvent(UnturnedPlayer.FromPlayer(__instance.player), item.id, ref allow);
            return allow;
        }
        catch
        {
            return true;
        }
    }
}