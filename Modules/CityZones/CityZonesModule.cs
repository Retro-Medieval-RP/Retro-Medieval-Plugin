using System.Collections;
using RetroMedieval.Modules;
using RetroMedieval.Modules.Attributes;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using UnityEngine;
using Zones.Events;

namespace CityZones
{
    [ModuleInformation("CityZones")]
    [ModuleConfiguration<CityZonesConfiguration>("CityZonesConfiguration")]
    [ModuleStorage<CitiesStorage>("Cities")]
    public class CityZonesModule(string directory) : Module(directory)
    {
        public override void Load()
        {
            ZoneEnterEventPublisher.ZoneEnterEvent += OnZoneEnter;
        }

        public override void Unload()
        {
            ZoneEnterEventPublisher.ZoneEnterEvent -= OnZoneEnter;
        }

        private void OnZoneEnter(ZoneEnterEventArgs e)
        {
            if (!GetStorage<CitiesStorage>(out var storage))
            {
                return;
            }

            if (!GetConfiguration<CityZonesConfiguration>(out var config))
            {
                return;
            }

            if (!storage.CityExists(e.Zone.ZoneName))
            {
                return;
            }
            
            var city = storage.GetCity(e.Zone.ZoneName);
            EffectManager.sendUIEffect(config.ID, 15543, e.Player.Player.channel.GetOwnerTransportConnection(), false);
            EffectManager.sendUIEffectText(15543, e.Player.Player.channel.GetOwnerTransportConnection(), false, "Text", $"<color=#b25151>{city.WelcomeMessage}</color>{(string.IsNullOrEmpty(city.TerritoryMessage) ? "" : $"\n{city.WelcomeMessage}")}");
            RetroMedieval.Main.Instance.StartCoroutine(ClearCity(e.Player.CSteamID.m_SteamID, 5));
        }

        private IEnumerator ClearCity(ulong playerId, int clearTime)
        {
            yield return new WaitForSeconds(clearTime);
            
            if (UnturnedPlayer.FromCSteamID(new CSteamID(playerId)) == null)
            {
                yield break;
            }
            
            var player = UnturnedPlayer.FromCSteamID(new CSteamID(playerId));
                
            if (!GetConfiguration<CityZonesConfiguration>(out var config))
            {
                yield break;
            }
            
            EffectManager.askEffectClearByID(config.ID, player.Player.channel.GetOwnerTransportConnection());
        }
    }
}