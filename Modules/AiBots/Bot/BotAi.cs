using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AiBots.Threads;
using Cysharp.Threading.Tasks;
using HarmonyLib;
using Pathfinding;
using Rocket.Unturned.Player;
using SDG.NetTransport;
using SDG.Unturned;
using Steamworks;
using UnityEngine;

namespace AiBots.Bot;

public class BotAi : MonoBehaviour
{
    private static readonly FieldInfo SBattlEyeId = AccessTools.Field(typeof(SteamPlayer), "battlEyeId");
    private Seeker _seeker = null;
    private Path _path = null;
    private Player _target = null;
    private float _nextWaypointDistance = 3f;
    private int _currentWaypoint = 0;
    private bool _reachedEndOfPath;
    private bool _shooting = false;
    private float _nextFire;
    private bool _hunter = false;
    private float _speed = 4f;
    private byte _damage = 5;
    private int _respawn = 120;
    private Vector3 _spawnpoint;

    public CSteamID Id { get; private set; }

    public ITransportConnection TransportConnection { get; private set; }

    public List<ushort> Layout { get; set; }

    public List<ushort> Drop { get; set; }

    public Player Player { get; private set; }

    public BotUserSim Simulation { get; set; }

    public int BattleyeId { get; private set; }

    public bool Hunter
    {
        get => _hunter;
        set => _hunter = value;
    }

    public void Prepare(CSteamID id, Vector3 spawnPoint)
    {
        Id = id;
        TransportConnection = new BotTransportConnection() as ITransportConnection;
        _seeker = gameObject.AddComponent<Seeker>();
        _spawnpoint = spawnPoint;
        CreateBot();
        PreventKick();
        BattleyeId = (int)SBattlEyeId.GetValue(Player.channel.owner);
    }

    private async void CreateBot()
    {
        var id1 = Id;
        var num = (Provider.clients).Count(e => e.playerID.characterName.StartsWith("RETRO_BOT_"));
        var str1 = "RETRO_BOT_" + num;
        num = Provider.clients.Count(e => e.playerID.characterName.StartsWith("RETRO_BOT_"));
        var str2 = "RETRO_BOT_" + num;
        num = Provider.clients.Count(e => e.playerID.characterName.StartsWith("RETRO_BOT_"));
        var str3 = "RETRO_BOT_" + num;
        var id2 = Id;
        var steamPlayerId = new SteamPlayerID(id1, 0, str1, str2, str3, id2);
        var steamPending = new SteamPending(
            TransportConnection,
            steamPlayerId,
            true,
            0,
            0,
            0,
            Color.white,
            Color.white,
            Color.white,
            false,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            [],
            0,
            "english",
            new CSteamID(0),
            EClientPlatform.Linux
        );
        
        PrepareInventoryDetails(steamPending);
        Provider.pending.Add(steamPending);
        Provider.accept(steamPending);
        var botPlayer = Provider.clients.LastOrDefault()?.player;
        Player = botPlayer;
        await DelayedRemoveRigidbody(botPlayer);
        Simulation = new BotUserSim(botPlayer);
        steamPlayerId = null;
        steamPending = null;
        botPlayer = null;
    }

    private void PrepareInventoryDetails(SteamPending pending)
    {
        pending.shirtItem = (int)pending.packageShirt;
        pending.pantsItem = (int)pending.packagePants;
        pending.hatItem = (int)pending.packageHat;
        pending.backpackItem = (int)pending.packageBackpack;
        pending.vestItem = (int)pending.packageVest;
        pending.maskItem = (int)pending.packageMask;
        pending.glassesItem = (int)pending.packageGlasses;
        pending.skinItems =
        [
            pending.shirtItem,
            pending.pantsItem,
            pending.hatItem,
            pending.backpackItem,
            pending.vestItem,
            pending.maskItem,
            pending.glassesItem
        ];
        pending.skinTags = [];
        pending.skinDynamicProps = [];
    }
    
    private async UniTask DelayedRemoveRigidbody(Player player)
    {
        await UniTask.DelayFrame(1);
        await UniTask.SwitchToMainThread();
        var movement = player.movement;
        movement.transform.DestroyRigidbody();
    }

    private async void PreventKick() => await PreventKick_Hook();

    private async Task PreventKick_Hook()
    {
        var time = (int) Math.Floor(Provider.configData.Server.Timeout_Game_Seconds * 0.5);
        while (true)
        {
            Player.channel.owner.timeLastPacketWasReceivedFromClient = Time.realtimeSinceStartup;
            await Task.Delay(time);
        }
    }
    
    private void Update()
    {
        if (_path == null || !_hunter || Player.life.isDead)
            return;
        _reachedEndOfPath = false;
        float num1;
        while (true)
        {
            num1 = Vector3.Distance(Player.transform.position, _path.vectorPath[_currentWaypoint]);
            if (num1 < (double)_nextWaypointDistance)
            {
                if (_currentWaypoint + 1 < _path.vectorPath.Count)
                    ++_currentWaypoint;
                else
                    break;
            }
            else
                goto label_7;
        }
        
        _reachedEndOfPath = true;
        label_7:
        var num2 = _reachedEndOfPath ? Mathf.Sqrt(num1 / _nextWaypointDistance) : 1f;
        var vector31 = _path.vectorPath[_currentWaypoint] - Player.transform.position;
        var vector3_2 = vector31.normalized * (_speed * num2);
        var quaternion = Quaternion.LookRotation(vector3_2);
        Simulation.SetRotation(57.29578f * Mathf.Atan2((float) (2.0 * quaternion.y * quaternion.w - 2.0 * quaternion.x * quaternion.z), (float) (1.0 - 2.0 * quaternion.y * quaternion.y - 2.0 * quaternion.z * quaternion.z)), 90f, 0.0f);
        Simulation.Sprint = true;
        Player.movement.controller.SimpleMove(vector3_2);
    }
    
    public async UniTask Damage(UnturnedPlayer damager, int second)
    {
        await UniTask.Delay(second * 1000);
        await UniTask.SwitchToMainThread();
        if (_target == null || Player.life.isDead || (long) _target.channel.owner.playerID.steamID.m_SteamID != (long) damager.CSteamID.m_SteamID || Time.time < (double) _nextFire || Player.equipment.state[10] <= 0)
            return;
        var direction = damager.Position - Player.transform.position;
        var q = Quaternion.LookRotation(direction);
        var yaw = 57.29578f * Mathf.Atan2((float) (2.0 * q.y * q.w - 2.0 * q.x * q.z), (float) (1.0 - 2.0 * q.y * q.y - 2.0 * q.z * q.z));
        Simulation.SetRotation(yaw, 90f, 0.0f);
        var gun = Player.equipment.useable as UseableGun;
        _nextFire = Time.time + 1f / gun!.equippedGunAsset.firerate;
        typeof (UseableGun).GetMethod("fire", BindingFlags.Instance | BindingFlags.NonPublic)?.Invoke(gun, []);
        damager.Damage(_damage, damager.Position, (EDeathCause) 6, (ELimb) 13, Player.channel.owner.playerID.steamID);
    }
    
    public async UniTask Respawn()
    {
        await UniTask.Delay(_respawn * 1000);
        await UniTask.SwitchToMainThread();
        Player.life.ServerRespawn(false);
        Player.teleportToLocationUnsafe(_spawnpoint, 90f);
        var unturnedPlayer = UnturnedPlayer.FromPlayer(Player);
        ClearInventory(Player);
        foreach (var item in Layout)
        {
            unturnedPlayer.GiveItem(item, 1);
        }
        Player.life.serverModifyHealth(100);
    }
    
    private void ClearInventory(Player player)
    {
        PlayerInventory playerInv = player.inventory;
        player.channel.send("tellSlot", (ESteamCall) 1, 0, [
            (byte) 0,
            (byte) 0, Array.Empty<byte>()
        ]);
        player.channel.send("tellSlot", (ESteamCall) 1, 0, [
            (byte) 1,
            (byte) 0, Array.Empty<byte>()
        ]);
        for (byte index1 = 0; index1 < PlayerInventory.PAGES; ++index1)
        {
            if (index1 != PlayerInventory.AREA)
            {
                byte itemCount = playerInv.getItemCount(index1);
                for (byte index2 = 0; index2 < itemCount; ++index2)
                    playerInv.removeItem(index1, 0);
            }
        }

        player.clothing.askWearBackpack(0, 0, [], true);
        Action();
        player.clothing.askWearGlasses(0, 0, [], true);
        Action();
        player.clothing.askWearHat(0, 0, [], true);
        Action();
        player.clothing.askWearPants(0, 0, [], true);
        Action();
        player.clothing.askWearMask(0, 0, [], true);
        Action();
        player.clothing.askWearShirt(0, 0, [], true);
        Action();
        player.clothing.askWearVest(0, 0, [], true);
        Action();
        return;

        void Action()
        {
            for (byte index = 0; index < playerInv.getItemCount(2); ++index) playerInv.removeItem(2, 0);
        }
    }
}