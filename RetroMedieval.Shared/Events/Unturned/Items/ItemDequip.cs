using Rocket.Unturned.Player;
using SDG.Unturned;

namespace RetroMedieval.Shared.Events.Unturned.Items
{
    public class ItemDequipEventArgs
    {
        public UnturnedPlayer Player { get; set; }
        public ItemJar Item { get; set; }
    }

    public static class ItemDequipEventPublisher
    {
        public delegate void ItemDequipEventHandler(ItemDequipEventArgs e, ref bool allow);

        public static event ItemDequipEventHandler ItemDequipEvent;

        public static void RaiseEvent(UnturnedPlayer player, ItemJar item, ref bool allow) =>
            ItemDequipEvent?.Invoke(new ItemDequipEventArgs()
            {
                Player = player,
                Item = item
            }, ref allow);
    }
}