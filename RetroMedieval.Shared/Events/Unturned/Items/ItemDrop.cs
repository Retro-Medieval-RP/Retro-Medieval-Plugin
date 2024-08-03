using Rocket.Unturned.Player;
using SDG.Unturned;

namespace RetroMedieval.Shared.Events.Unturned.Items
{
    public class ItemDropEventArgs
    {
        public byte Page { get; set; }
        public byte X { get; set; }
        public byte Y { get; set; }

        public UnturnedPlayer Player { get; set; }
        public ItemJar Item { get; set; }
    }

    public static class ItemDropEventPublisher
    {
        public delegate void ItemDropEventHandler(ItemDropEventArgs e, ref bool allow);

        public static event ItemDropEventHandler ItemDropEvent;

        public static void RaiseEvent(byte page, byte x, byte y, UnturnedPlayer player, ItemJar item, ref bool allow) =>
            ItemDropEvent?.Invoke(new ItemDropEventArgs
            {
                Page = page,
                X = x,
                Y = y,
                Player = player,
                Item = item
            }, ref allow);
    }
}