using System;
using System.Collections.Generic;

namespace TheLostLand.Core.Modules.Configuration;

public sealed class ConfigurationManager : IManager<Config>
{
    private List<Config> _items { get; set; }
    public IReadOnlyList<Config> Items => _items;

    public bool Get(Predicate<Config> condition, out Config item)
    {
        if (!_items.Exists(condition))
        {
            item = default;
            return false;
        }
        
        item = _items.Find(condition);
        return true;
    }

    public void Add(Config item)
    {
        _items.Add(item);
    }

    public void Remove(Config item)
    {
        _items.Remove(item);
    }
}