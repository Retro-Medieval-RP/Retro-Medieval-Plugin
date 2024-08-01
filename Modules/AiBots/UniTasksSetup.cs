using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.LowLevel;

namespace AiBots;

internal class UniTasksSetup
{
    private static bool m_Initialized { get; set; }

    internal static void CheckInit()
    {
        if (m_Initialized)
            return;
        m_Initialized = true;
        Init();
    }

    private static void Init()
    {
        if (IsOpenmodPresent())
            return;
        typeof(PlayerLoopHelper).GetField("unitySynchronizationContext", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue((object) null, (object) SynchronizationContext.Current);
        (typeof(PlayerLoopHelper).GetField("mainThreadId", BindingFlags.Static | BindingFlags.NonPublic) ?? throw new Exception("Could not find PlayerLoopHelper.mainThreadId field")).SetValue((object) null, (object) Thread.CurrentThread.ManagedThreadId);
        var currentPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();
        PlayerLoopHelper.Initialize(ref currentPlayerLoop);
    }

    public static bool IsOpenmodPresent() => AppDomain.CurrentDomain.GetAssemblies().Any(x => x.GetName().Name == "OpenMod.Core");
}