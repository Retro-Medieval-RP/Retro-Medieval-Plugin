using System;

namespace TheLostLand.Core.Utils;

public class Padlock<T> where T : class
{
    private static T _padLockInstance;
    public static T Instance => _padLockInstance ??= Activator.CreateInstance<T>();
    
    public static void SetInstance(T instance) => _padLockInstance = instance;
}