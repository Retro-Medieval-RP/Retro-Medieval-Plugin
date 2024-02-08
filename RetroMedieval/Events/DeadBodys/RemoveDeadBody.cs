using UnityEngine;

namespace RetroMedieval.Events.DeadBodys;

public class RemoveDeadBodyEventArgs
{
    public Vector3 Location { get; set; }
}

public static class RemoveDeadBodyEventPublisher
{
    public delegate void RemoveDeadBodyEventHandler(RemoveDeadBodyEventArgs e);

    public static event RemoveDeadBodyEventHandler RemoveDeadBodyEvent;

    internal static void RaiseEvent(Vector3 location) =>
        RemoveDeadBodyEvent?.Invoke(new RemoveDeadBodyEventArgs()
        {
            Location = location
        });
}