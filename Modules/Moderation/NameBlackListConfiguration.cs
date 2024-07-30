using System.Collections.Generic;
using RetroMedieval.Modules.Configuration;

namespace Moderation;

internal class NameBlackListConfiguration : IConfig
{
    public bool BlacklistNonEnglishCharacters { get; set; }
    public bool BlacklistSpecialCharacters { get; set; }
    public List<string> BlacklistFilter { get; set; }
    
    public void LoadDefaults()
    {
        BlacklistNonEnglishCharacters = true;
        BlacklistSpecialCharacters = false;
        
        BlacklistFilter =
        [
            @"twitch.tv",
            @"youtube"
        ];
    }
}