using Rocket.Unturned.Player;
using SDG.Unturned;

namespace RetroMedieval.Shared.Events.Unturned.Storage
{
    public class CloseStorageEventArgs
    {
        public UnturnedPlayer Player { get; set; }
        public InteractableStorage StorageClosed { get; set; }
    }

    public static class CloseStorageEventPublisher
    {
        public delegate void CloseStorageEventHandler(CloseStorageEventArgs e, ref bool allow);

        public static event CloseStorageEventHandler CloseStorageEvent;

        public static void RaiseEvent(UnturnedPlayer player, InteractableStorage storage, ref bool allow) =>
            CloseStorageEvent?.Invoke(new CloseStorageEventArgs
            {
                Player = player,
                StorageClosed = storage
            }, ref allow);
    }
}

