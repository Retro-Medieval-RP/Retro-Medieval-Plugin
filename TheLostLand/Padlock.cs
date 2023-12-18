using System;

namespace TheLostLand;

public class Padlock<T>
{
    private static T PadLockInstance;
    public static T Instance => PadLockInstance ??= Activator.CreateInstance<T>();

    public void SetInstance(T instance) => PadLockInstance = instance;
}