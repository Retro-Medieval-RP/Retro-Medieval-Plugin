using Rocket.Unturned.Player;
using SDG.Unturned;

namespace RetroMedieval.Shared.Events.Unturned.Items
{
    public class ItemEquipEventArgs
    {
        public ItemJar Item { get; set; }
        public UnturnedPlayer Player { get; set; }
    }

    public static class ItemEquipEventPublisher
    {
        public delegate void ItemEquipEventHandler(ItemEquipEventArgs e, ref bool allow);

        public static event ItemEquipEventHandler ItemEquipEvent;

        public static void RaiseEvent(UnturnedPlayer player, ItemJar item, ref bool allow) =>
            ItemEquipEvent?.Invoke(new ItemEquipEventArgs()
            {
                Item = item,
                Player = player
            }, ref allow);
    }
}