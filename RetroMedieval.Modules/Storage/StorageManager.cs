using System;
using System.Collections.Generic;
using System.IO;
using RetroMedieval.Modules.Configuration;
using RetroMedieval.Utils;

namespace RetroMedieval.Modules.Storage;

internal sealed class StorageManager : Padlock<StorageManager>, IManager<Storage>
{
    private List<Storage> _items { get; } = [];
    public IReadOnlyList<Storage> Items => _items;

    public StorageManager() => 
        SavingConfiguration.LoadedConfiguration(Path.Combine(ModuleLoader.Instance.ModuleDirectory));

    public bool Get(Predicate<Storage> condition, out Storage item)
    {
        if (!_items.Exists(condition))
        {
            item = default!;
            return false;
        }
        
        item = _items.Find(condition);
        return true;
    }

    public static SavingConfiguration GetSavingConfig()
    {
        if (!ConfigurationManager.Instance.Get((x => x.Config.GetType() == typeof(SavingConfiguration)), out var config))
        {
            return default!;
        }

        return (config.Config as SavingConfiguration)!;
    }

    public void Add(Storage item) => 
        _items.Add(item);

    public void Remove(Storage item) => 
        _items.Remove(item);

    public bool Has(string name) => 
        _items.Exists(x => x.StorageName == name);

    public void Clear() => 
        _items.Clear();
}