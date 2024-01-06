namespace TheLostLand.Utils;

public class Padlock<T>
{
    private static T _padLockInstance;
    public static T Instance => _padLockInstance ??= Activator.CreateInstance<T>();

    public void SetInstance(T instance) => _padLockInstance = instance;
}