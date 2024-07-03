using System;
using HarmonyLib;
using RetroMedieval.Events.Unturned.CloathingDequip;
using RetroMedieval.Events.Unturned.ClothingEquip;
using Rocket.Unturned.Player;
using SDG.Unturned;

namespace RetroMedieval.Patches;

[HarmonyPatch(typeof(PlayerClothing))]
[HarmonyPatch("ReceiveWearShirt")]
internal class EquipShirtPatch
{
    public static void Prefix(Guid id, byte quality, byte[] state, bool playEffect, PlayerClothing instance)
    {
        try
        {
            if (instance.player == null)
            {
                return;
            }

            if (id == Guid.Empty)
            {
                ShirtDequippedEventPublisher.RaiseEvent(UnturnedPlayer.FromPlayer(instance.player), instance.player.clothing.shirt);
                ClothingDequipEventPublisher.RaiseEvent(UnturnedPlayer.FromPlayer(instance.player), instance.player.clothing.shirt);
                return;
            }

            var item = Assets.find(id);
            ShirtEquippedEventPublisher.RaiseEvent(UnturnedPlayer.FromPlayer(instance.player), item.id);
            ClothingEquipEventPublisher.RaiseEvent(UnturnedPlayer.FromPlayer(instance.player), item.id);
        }
        catch
        {
            return;
        }
    }
}

[HarmonyPatch(typeof(PlayerClothing))]
[HarmonyPatch("ReceiveWearPants")]
internal class EquipPantsPatch
{
    public static void Prefix(Guid id, byte quality, byte[] state, bool playEffect, PlayerClothing instance)
    {
        try
        {
            if (instance.player == null)
            {
                return;
            }

            if (id == Guid.Empty)
            {
                PantsDequippedEventPublisher.RaiseEvent(UnturnedPlayer.FromPlayer(instance.player), instance.player.clothing.pants);
                ClothingDequipEventPublisher.RaiseEvent(UnturnedPlayer.FromPlayer(instance.player), instance.player.clothing.pants);
                return;
            }

            var item = Assets.find(id);
            PantsEquippedEventPublisher.RaiseEvent(UnturnedPlayer.FromPlayer(instance.player), item.id);
            ClothingEquipEventPublisher.RaiseEvent(UnturnedPlayer.FromPlayer(instance.player), item.id);
        }
        catch
        {
            return;
        }
    }
}

[HarmonyPatch(typeof(PlayerClothing))]
[HarmonyPatch("ReceiveWearHat")]
internal class EquipHatPatch
{
    public static void Prefix(Guid id, byte quality, byte[] state, bool playEffect, PlayerClothing instance)
    {
        try
        {
            if (instance.player == null)
            {
                return;
            }

            if (id == Guid.Empty)
            {
                HatDequippedEventPublisher.RaiseEvent(UnturnedPlayer.FromPlayer(instance.player), instance.player.clothing.hat);
                ClothingDequipEventPublisher.RaiseEvent(UnturnedPlayer.FromPlayer(instance.player), instance.player.clothing.hat);
                return;
            }

            var item = Assets.find(id);
            HatEquippedEventPublisher.RaiseEvent(UnturnedPlayer.FromPlayer(instance.player), item.id);
            ClothingEquipEventPublisher.RaiseEvent(UnturnedPlayer.FromPlayer(instance.player), item.id);
        }
        catch
        {
            return;
        }
    }
}

[HarmonyPatch(typeof(PlayerClothing))]
[HarmonyPatch("ReceiveWearBackpack")]
internal class EquipBackpackPatch
{
    public static void Prefix(Guid id, byte quality, byte[] state, bool playEffect, PlayerClothing instance)
    {
        try
        {
            if (instance.player == null)
            {
                return;
            }

            if (id == Guid.Empty)
            {
                BackpackDequippedEventPublisher.RaiseEvent(UnturnedPlayer.FromPlayer(instance.player), instance.player.clothing.backpack);
                ClothingDequipEventPublisher.RaiseEvent(UnturnedPlayer.FromPlayer(instance.player), instance.player.clothing.backpack);
                return;
            }

            var item = Assets.find(id);
            BackpackEquippedEventPublisher.RaiseEvent(UnturnedPlayer.FromPlayer(instance.player), item.id);
            ClothingEquipEventPublisher.RaiseEvent(UnturnedPlayer.FromPlayer(instance.player), item.id);
        }
        catch
        {
            return;
        }
    }
}

[HarmonyPatch(typeof(PlayerClothing))]
[HarmonyPatch("ReceiveWearVest")]
internal class EquipVestPatch
{
    public static void Prefix(Guid id, byte quality, byte[] state, bool playEffect, PlayerClothing instance)
    {
        try
        {
            if (instance.player == null)
            {
                return;
            }

            if (id == Guid.Empty)
            {
                VestDequippedEventPublisher.RaiseEvent(UnturnedPlayer.FromPlayer(instance.player), instance.player.clothing.vest);
                ClothingDequipEventPublisher.RaiseEvent(UnturnedPlayer.FromPlayer(instance.player), instance.player.clothing.vest);
                return;
            }

            var item = Assets.find(id);
            VestEquippedEventPublisher.RaiseEvent(UnturnedPlayer.FromPlayer(instance.player), item.id);
            ClothingEquipEventPublisher.RaiseEvent(UnturnedPlayer.FromPlayer(instance.player), item.id);
        }
        catch
        {
            return;
        }
    }
}

[HarmonyPatch(typeof(PlayerClothing))]
[HarmonyPatch("ReceiveWearMask")]
internal class EquipMaskPatch
{
    public static void Prefix(Guid id, byte quality, byte[] state, bool playEffect, PlayerClothing instance)
    {
        try
        {
            if (instance.player == null)
            {
                return;
            }

            if (id == Guid.Empty)
            {
                MaskDequippedEventPublisher.RaiseEvent(UnturnedPlayer.FromPlayer(instance.player), instance.player.clothing.mask);
                ClothingDequipEventPublisher.RaiseEvent(UnturnedPlayer.FromPlayer(instance.player), instance.player.clothing.mask);
                return;
            }

            var item = Assets.find(id);
            MaskEquippedEventPublisher.RaiseEvent(UnturnedPlayer.FromPlayer(instance.player), item.id);
            ClothingEquipEventPublisher.RaiseEvent(UnturnedPlayer.FromPlayer(instance.player), item.id);
        }
        catch
        {
            return;
        }
    }
}

[HarmonyPatch(typeof(PlayerClothing))]
[HarmonyPatch("ReceiveWearGlasses")]
internal class EquipGlassesPatch
{
    public static void Prefix(Guid id, byte quality, byte[] state, bool playEffect, PlayerClothing instance)
    {
        try
        {
            if (instance.player == null)
            {
                return;
            }

            if (id == Guid.Empty)
            {
                GlassesDequippedEventPublisher.RaiseEvent(UnturnedPlayer.FromPlayer(instance.player), instance.player.clothing.glasses);
                ClothingDequipEventPublisher.RaiseEvent(UnturnedPlayer.FromPlayer(instance.player), instance.player.clothing.glasses);
                return;
            }

            var item = Assets.find(id);
            GlassesEquippedEventPublisher.RaiseEvent(UnturnedPlayer.FromPlayer(instance.player), item.id);
            ClothingEquipEventPublisher.RaiseEvent(UnturnedPlayer.FromPlayer(instance.player), item.id);
        }
        catch
        {
            return;
        }
    }
}