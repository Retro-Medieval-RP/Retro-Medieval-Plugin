using System;
using System.Linq;
using HarmonyLib;
using RetroMedieval.Modules;
using SDG.Framework.Modules;
using SDG.Unturned;

namespace AiBots.Patches;

[HarmonyPatch(typeof (Provider))]
[HarmonyPatch("battlEyeServerKickPlayer")]
public class ProviderPatch
{
    [HarmonyPrefix]
    public static bool Prefix(int playerID) => ModuleLoader.Instance.GetModule<AiBotsModule>(out var module) && module.ActiveBots.All(x => x.BattleyeId != playerID);
}