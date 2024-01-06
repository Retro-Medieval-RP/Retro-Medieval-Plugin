namespace TheLostLand.Utils;

public class Padlock<T>
{
    private static T _padLockInstance;
    internal static T Instance => _padLockInstance ??= Activator.CreateInstance<T>();

    internal static void SetInstance(T instance) => _padLockInstance = instance;
}