using System;
using System.Collections.Generic;
using System.Linq;

namespace RetroMedieval.Utils;

public class Picker<T>
{
    private struct Entry
    {
        public double AccumulatedWeight;
        public T Item;
    }

    private readonly List<Entry> _entries = [];
    private double _accumulatedWeight;
    private Random Random { get; } = new();

    public void AddEntry(T item, double weight)
    {
        _accumulatedWeight += weight;
        _entries.Add(new Entry { Item = item, AccumulatedWeight = _accumulatedWeight });
    }

    public T GetRandom()
    {
        var r = Random.NextDouble() * _accumulatedWeight;

        foreach (var entry in _entries.Where(entry => entry.AccumulatedWeight >= r))
        {
            return entry.Item;
        }

        return default;
    }
}