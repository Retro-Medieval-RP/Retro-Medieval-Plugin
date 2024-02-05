namespace RetroMedieval.Modules.Storage;

internal class Storage(string name, IStorage storage)
{
    public string StorageName { get; set; } = name;
    public IStorage Store { get; set; } = storage;
}