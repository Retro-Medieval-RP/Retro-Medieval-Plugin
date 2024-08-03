using Rocket.Unturned.Player;
using SDG.Unturned;

namespace RetroMedieval.Shared.Events.Unturned.Items
{
    public class ItemUpdateQualityEventArgs
    {
        public byte Page { get; set; }
        public byte Index { get; set; }
        public byte Quality { get; set; }
        
        public UnturnedPlayer Player { get; set; }
        public ItemJar Item { get; set; }
    }

    public static class ItemUpdateQualityEventPublisher
    {
        public delegate void ItemUpdateQualityEventHandler(ItemUpdateQualityEventArgs e, ref bool allow);

        public static event ItemUpdateQualityEventHandler ItemUpdateQualityEvent;

        public static void RaiseEvent(byte page, byte index, byte quality, UnturnedPlayer player, ItemJar item, ref bool allow) =>
            ItemUpdateQualityEvent?.Invoke(new ItemUpdateQualityEventArgs
            {
                Page = page,
                Index = index,
                Quality = quality,
                Player = player,
                Item = item
            }, ref allow);
    }
}