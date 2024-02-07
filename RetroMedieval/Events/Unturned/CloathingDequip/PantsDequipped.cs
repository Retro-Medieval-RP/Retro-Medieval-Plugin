using Rocket.Unturned.Player;

namespace RetroMedieval.Events.Unturned.CloathingDequip
{
    public class PantsDequippedEventArgs
    {
        public UnturnedPlayer Player { get; set; }
        public ushort DequippedClothingID { get; set; }
    }

    public static class PantsDequippedEventPublisher
    {
        public delegate void PantsDequippedEventHandler(PantsDequippedEventArgs e);

        public static event PantsDequippedEventHandler PantsDequippedEvent;

        internal static void RaiseEvent(UnturnedPlayer player, ushort id) =>
            PantsDequippedEvent?.Invoke(new PantsDequippedEventArgs()
            {
                Player = player,
                DequippedClothingID = id
            });
    }
}

