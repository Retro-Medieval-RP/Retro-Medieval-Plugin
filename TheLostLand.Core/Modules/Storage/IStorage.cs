namespace TheLostLand.Core.Modules.Storage;

public interface IStorage
{
    bool Load(string file_path);
    void Save();
}