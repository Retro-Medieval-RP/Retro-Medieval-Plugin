using Rocket.Unturned.Player;
using SDG.Unturned;

namespace RetroMedieval.Shared.Events.Unturned.Items
{
    public class ItemUpdateInvStateEventArgs
    {
        public byte Page { get; set; }
        public byte Index { get; set; }
        public byte[] State { get; set; }
        
        public UnturnedPlayer Player { get; set; }
        public ItemJar Item { get; set; }
    }

    public static class ItemUpdateInvStateEventPublisher
    {
        public delegate void ItemUpdateInvStateEventHandler(ItemUpdateInvStateEventArgs e, ref bool allow);

        public static event ItemUpdateInvStateEventHandler ItemUpdateInvStateEvent;

        public static void RaiseEvent(byte page, byte index, byte[] state, UnturnedPlayer player, ItemJar item, ref bool allow) =>
            ItemUpdateInvStateEvent?.Invoke(new ItemUpdateInvStateEventArgs
            {
                Page = page,
                Index = index,
                State = state,
                Player = player,
                Item = item
            }, ref allow);
    }
}

