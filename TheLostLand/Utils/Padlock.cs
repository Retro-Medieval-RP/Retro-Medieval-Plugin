namespace TheLostLand.Utils;

public class Padlock<T> where T : class
{
    private static T _padLockInstance;
    internal static T Instance => _padLockInstance ??= Activator.CreateInstance(typeof(T), true) as T;

    internal static void SetInstance(T instance) => _padLockInstance = instance;
}