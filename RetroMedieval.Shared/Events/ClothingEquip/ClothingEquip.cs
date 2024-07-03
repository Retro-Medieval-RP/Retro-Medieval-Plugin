using Rocket.Unturned.Player;

namespace RetroMedieval.Shared.Events.ClothingEquip;

public class ClothingEquipEventArgs
{
    public UnturnedPlayer Player { get; set; }
    public ushort ClothingItem { get; set; }
}

public static class ClothingEquipEventPublisher
{
    public delegate void ClothingEquipEventHandler(ClothingEquipEventArgs e);

    public static event ClothingEquipEventHandler ClothingEquipEvent;

    public static void RaiseEvent(UnturnedPlayer player, ushort item) =>
        ClothingEquipEvent?.Invoke(new ClothingEquipEventArgs
        {
            Player = player,
            ClothingItem = item
        });
}