﻿using System;
using System.Collections.Generic;
using TheLostLand.Core.Utils;

namespace TheLostLand.Core.Modules.Configuration;

public sealed class ConfigurationManager : Padlock<ConfigurationManager>, IManager<Configuration>
{
    private List<Configuration> _items { get; set; }
    public IReadOnlyList<Configuration> Items => _items;

    public bool Get(Predicate<Configuration> condition, out Configuration item)
    {
        if (!_items.Exists(condition))
        {
            item = default;
            return false;
        }
        
        item = _items.Find(condition);
        return true;
    }

    public void Add(Configuration item) => 
        _items.Add(item);

    public void Remove(Configuration item) => 
        _items.Remove(item);

    public bool Has(string name) => 
        _items.Exists(x => x.ConfigName == name);
}