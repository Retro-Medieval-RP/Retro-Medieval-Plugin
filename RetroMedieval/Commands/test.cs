using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Player;
using SDG.Unturned;

namespace RetroMedieval.Commands;

public class Test : IRocketCommand
{
    public void Execute(IRocketPlayer caller, string[] command)
    {
        EffectManager.sendEffect(65000, ((UnturnedPlayer)caller).CSteamID, ((UnturnedPlayer)caller).Position);
    }

    public AllowedCaller AllowedCaller => AllowedCaller.Player;
    public string Name => "send";
    public string Help => "";
    public string Syntax => "";
    public List<string> Aliases => [];
    public List<string> Permissions => [];
}