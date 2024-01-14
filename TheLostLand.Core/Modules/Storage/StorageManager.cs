﻿using System;
using System.Collections.Generic;
using TheLostLand.Core.Utils;

namespace TheLostLand.Core.Modules.Storage;

public sealed class StorageManager : Padlock<StorageManager>, IManager<Storage>
{
    private List<Storage> _items { get; set; }
    public IReadOnlyList<Storage> Items => _items;
    
    public bool Get(Predicate<Storage> condition, out Storage item)
    {
        if (!_items.Exists(condition))
        {
            item = default;
            return false;
        }
        
        item = _items.Find(condition);
        return true;
    }

    public void Add(Storage item) => 
        _items.Add(item);

    public void Remove(Storage item) => 
        _items.Remove(item);

    public bool Has(string name) => 
        _items.Exists(x => x.StorageName == name);
}