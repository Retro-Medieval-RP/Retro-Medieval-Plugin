using Rocket.Unturned.Player;

namespace RetroMedieval.Events.Unturned.CloathingDequip
{
    public class BackpackDequippedEventArgs
    {
        public UnturnedPlayer Player { get; set; }
        public ushort DequippedClothingID { get; set; }
    }

    public static class BackpackDequippedEventPublisher
    {
        public delegate void BackpackDequippedEventHandler(BackpackDequippedEventArgs e);

        public static event BackpackDequippedEventHandler BackpackDequippedEvent;

        internal static void RaiseEvent(UnturnedPlayer player, ushort id) =>
            BackpackDequippedEvent?.Invoke(new BackpackDequippedEventArgs()
            {
                Player = player,
                DequippedClothingID = id
            });
    }
}

