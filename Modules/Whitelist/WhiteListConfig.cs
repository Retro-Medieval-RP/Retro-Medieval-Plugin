using System.Collections.Generic;
using RetroMedieval.Modules.Configuration;

namespace Whitelist;

internal class WhiteListConfig : IConfig
{
    public bool WhitelistEnabled { get; set; }
    public string DeniedMessage { get; set; }
    public List<ulong> WhitelistedUsers { get; set; }
    
    public void LoadDefaults()
    {
        WhitelistEnabled = true;
        DeniedMessage = "";
        WhitelistedUsers = [];
    }
}