using Rocket.Unturned.Player;
using SDG.Unturned;

namespace RetroMedieval.Shared.Events.Unturned.Items
{
    public class ItemSwapEventArgs
    {
        public byte FromPage { get; set; }
        public byte FromX { get; set; }
        public byte FromY { get; set; }
        public byte FromRot { get; set; }

        public byte ToPage { get; set; }
        public byte ToX { get; set; }
        public byte ToY { get; set; }
        public byte ToRot { get; set; }

        public UnturnedPlayer Player { get; set; }
        public ItemJar Item { get; set; }
    }

    public static class ItemSwapEventPublisher
    {
        public delegate void ItemSwapEventHandler(ItemSwapEventArgs e, ref bool allow);

        public static event ItemSwapEventHandler ItemSwapEvent;

        public static void RaiseEvent(byte fromPage, byte fromX, byte fromY, byte fromRot, byte toPage, byte toX,
            byte toY, byte toRot, UnturnedPlayer player, ItemJar item, ref bool allow) =>
            ItemSwapEvent?.Invoke(new ItemSwapEventArgs
            {
                FromPage = fromPage,
                FromX = fromX,
                FromY = fromY,
                FromRot = fromRot,
                ToPage = toPage,
                ToX = toX,
                ToY = toY,
                ToRot = toRot,
                Player = player,
                Item = item
            }, ref allow);
    }
}