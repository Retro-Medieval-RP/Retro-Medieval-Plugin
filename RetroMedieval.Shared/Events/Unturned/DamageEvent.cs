using SDG.Unturned;
using Steamworks;
using UnityEngine;

namespace RetroMedieval.Shared.Events.Unturned;

public class DamageEventEventArgs
{
    public byte Amount { get; set; }
    public Vector3 Ragdoll { get; set; }
    public EDeathCause Cause { get; set; }
    public ELimb Limb { get; set; }
    public CSteamID Killer { get; set; }
    public bool Track { get; set; }
    public ERagdollEffect RagdollEffect { get; set; }
    public bool CauseBleeding { get; set; }
    public Player Player { get; set; }
}

public static class DamageEventEventPublisher
{
    public delegate void DamageEventEventHandler(DamageEventEventArgs e, ref EPlayerKill kill, ref bool allow);

    public static event DamageEventEventHandler DamageEventEvent;

    public static void RaiseEvent(byte amount, Vector3 ragdoll, EDeathCause cause, ELimb limb, CSteamID killer, bool track, ERagdollEffect ragdollEffect, bool causeBleeding, Player player, ref EPlayerKill kill, ref bool allow) =>
        DamageEventEvent?.Invoke(new DamageEventEventArgs()
        {
            Amount = amount,
            Ragdoll = ragdoll,
            Cause = cause,
            Limb = limb,
            Killer = killer,
            Track = track,
            RagdollEffect = ragdollEffect,
            CauseBleeding = causeBleeding,
            Player = player
        }, ref kill, ref allow);
}

