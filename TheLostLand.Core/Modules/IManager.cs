using System;
using System.Collections.Generic;

namespace TheLostLand.Core.Modules;

public interface IManager<T>
{
    
    IReadOnlyList<T> Items { get; }
    
    public bool Get(Predicate<T> condition, out T item);
    
    public void Add(T item);
    public void Remove(T item);
}