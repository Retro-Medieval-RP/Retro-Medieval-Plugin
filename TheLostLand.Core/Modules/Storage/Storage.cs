namespace TheLostLand.Core.Modules.Storage;

public class Storage(string name, IStorage storage)
{
    public string StorageName { get; set; } = name;
    public IStorage Store { get; set; } = storage;
}