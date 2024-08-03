using Rocket.Unturned.Player;
using SDG.Unturned;

namespace RetroMedieval.Shared.Events.Unturned.Inventory
{
    public class ItemRemoveEventArgs
    {
        public byte Page { get; set; }
        public byte X { get; set; }
        public byte Y { get; set; }
        
        public UnturnedPlayer Player { get; set; }
        public ItemJar Item { get; set; }
    }

    public static class ItemRemoveEventPublisher
    {
        public delegate void ItemRemoveEventHandler(ItemRemoveEventArgs e, ref bool allow);

        public static event ItemRemoveEventHandler ItemRemoveEvent;

        public static void RaiseEvent(byte page, byte x, byte y, UnturnedPlayer player, ItemJar item, ref bool allow) =>
            ItemRemoveEvent?.Invoke(new ItemRemoveEventArgs
            {
                Page = page,
                X = x,
                Y = y,
                Player = player,
                Item = item
            }, ref allow);
    }
}