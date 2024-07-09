namespace RetroMedieval.Shared.Events.Unturned.Storage
{
    public class CloseStorageEventArgs
    {
    }

    public static class CloseStorageEventPublisher
    {
        public delegate void CloseStorageEventHandler(CloseStorageEventArgs e);

        public static event CloseStorageEventHandler CloseStorageEvent;

        internal static void RaiseEvent() =>
            CloseStorageEvent?.Invoke(new CloseStorageEventArgs());
    }
}

