using Rocket.Unturned.Player;
using SDG.Unturned;

namespace RetroMedieval.Shared.Events.Unturned.Inventory
{
    public class ItemAddEventArgs
    {
        public byte Page { get; set; }
        public byte X { get; set; }
        public byte Y { get; set; }
        public byte Rot { get; set; }
        
        public UnturnedPlayer Player { get; set; }
        public ItemJar Item { get; set; }
    }

    public static class ItemAddEventPublisher
    {
        public delegate void ItemAddEventHandler(ItemAddEventArgs e, ref bool allow);

        public static event ItemAddEventHandler ItemAddEvent;

        public static void RaiseEvent(byte page, byte x, byte y, byte rot, UnturnedPlayer player, ItemJar item, ref bool allow) =>
            ItemAddEvent?.Invoke(new ItemAddEventArgs
            {
                Page = page,
                X = x,
                Y = y,
                Rot = rot,
                Player = player,
                Item = item
            }, ref allow);
    }
}

