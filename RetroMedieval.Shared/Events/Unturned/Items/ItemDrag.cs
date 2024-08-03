using Rocket.Unturned.Player;
using SDG.Unturned;

namespace RetroMedieval.Shared.Events.Unturned.Items
{
    public class ItemDragEventArgs
    {
        public byte FromPage { get; set; }
        public byte FromX { get; set; }
        public byte FromY { get; set; }

        public byte ToPage { get; set; }
        public byte ToX { get; set; }
        public byte ToY { get; set; }
        public byte ToRot { get; set; }

        public UnturnedPlayer Player { get; set; }
        public ItemJar Item { get; set; }
    }

    public static class ItemDragEventPublisher
    {
        public delegate void ItemDragEventHandler(ItemDragEventArgs e, ref bool allow);

        public static event ItemDragEventHandler ItemDragEvent;

        public static void RaiseEvent(byte fromPage, byte fromX, byte fromY, byte toPage, byte toX, byte toY,
            byte toRot, UnturnedPlayer player, ItemJar item, ref bool allow) =>
            ItemDragEvent?.Invoke(new ItemDragEventArgs
            {
                FromPage = fromPage,
                FromX = fromX,
                FromY = fromY,
                ToPage = toPage,
                ToX = toX,
                ToY = toY,
                ToRot = toRot,
                Player = player,
                Item = item
            }, ref allow);
    }
}