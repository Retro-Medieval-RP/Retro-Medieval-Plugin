using Rocket.Unturned.Player;

namespace RetroMedieval.Shared.Events.Unturned.Inventory
{
    public class ClothingSizeEventArgs
    {
        public byte Page { get; set; }
        public byte NewWidth { get; set; }
        public byte NewHeight { get; set; }
        
        public UnturnedPlayer Player { get; set; }
    }

    public static class ClothingSizeEventPublisher
    {
        public delegate void ClothingSizeEventHandler(ClothingSizeEventArgs e, ref bool allow);

        public static event ClothingSizeEventHandler ClothingSizeEvent;

        public static void RaiseEvent(byte page, byte newWidth, byte newHeight, UnturnedPlayer player, ref bool allow) =>
            ClothingSizeEvent?.Invoke(new ClothingSizeEventArgs
            {
                Page = page,
                NewWidth = newWidth,
                NewHeight = newHeight,
                Player = player
            }, ref allow);
    }
}

