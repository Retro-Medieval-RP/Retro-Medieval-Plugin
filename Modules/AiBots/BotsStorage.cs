using System.Collections.Generic;
using AiBots.Bot;
using RetroMedieval.Savers.Json;

namespace AiBots;

internal class BotsStorage : JsonSaver<List<BotData>>
{
    public void AddBotData(BotData data)
    {
        StorageItem.Add(data);
        Save();
    }

    public void RemoveBot(ulong id)
    {
        StorageItem.RemoveAll(x => x.Id == id);
        Save();
    }
}