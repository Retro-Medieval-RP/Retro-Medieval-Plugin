using Rocket.Unturned.Player;
using SDG.Unturned;

namespace RetroMedieval.Shared.Events.Unturned.Items
{
    public class ItemUpdateAmountEventArgs
    {
        public byte Page { get; set; }
        public byte Index { get; set; }
        public byte Amount { get; set; }
        
        public UnturnedPlayer Player { get; set; }
        public ItemJar Item { get; set; }
    }

    public static class ItemUpdateAmountEventPublisher
    {
        public delegate void ItemUpdateAmountEventHandler(ItemUpdateAmountEventArgs e, ref bool allow);

        public static event ItemUpdateAmountEventHandler ItemUpdateAmountEvent;

        public static void RaiseEvent(byte page, byte index, byte amount, UnturnedPlayer player, ItemJar item, ref bool allow) =>
            ItemUpdateAmountEvent?.Invoke(new ItemUpdateAmountEventArgs
            {
                Page = page,
                Index = index,
                Amount = amount,
                Player = player,
                Item = item
            }, ref allow);
    }
}