using System.Reflection;
using HarmonyLib;
using Multiplayer.API;
using Verse;

namespace JetPack;

[StaticConstructorOnStartup]
internal static class MultiplayerSupport
{
    private static readonly Harmony harmony = new("rimworld.pelador.jetpack.multiplayersupport");

    static MultiplayerSupport()
    {
        if (!MP.enabled)
        {
            return;
        }

        MP.RegisterSyncMethod(typeof(JetPackApparel), "useJetPack");
        MP.RegisterSyncMethod(typeof(JetPackApparel), "toggleSlowBurn");
        MP.RegisterSyncMethod(typeof(JetPackApparel), "refuelJP");
        MP.RegisterSyncMethod(typeof(JetPackApparel), "dropFuelJP");
        MP.RegisterSyncMethod(typeof(JetPackApparel), "changeFuelJP");
        MP.RegisterSyncMethod(typeof(JetPackApparel), "debugUseJP");
        MethodInfo[] array =
        [
            AccessTools.Method(typeof(JPInjury), nameof(JPInjury.CheckForExplosion)),
            AccessTools.Method(typeof(JPInjury), nameof(JPInjury.DoJPRelatedInjury)),
            AccessTools.Method(typeof(JPInjury), "SetUpInjVars"),
            AccessTools.Method(typeof(JPSkyFaller), "JPImpact"),
            AccessTools.Method(typeof(JPSkyFaller), "JPIgnite")
        ];
        foreach (var methodInfo in array)
        {
            FixRNG(methodInfo);
        }
    }

    private static void FixRNG(MethodInfo method)
    {
        harmony.Patch(method, new HarmonyMethod(typeof(MultiplayerSupport), nameof(FixRNGPre)),
            new HarmonyMethod(typeof(MultiplayerSupport), nameof(FixRNGPos)));
    }

    private static void FixRNGPre()
    {
        Rand.PushState(Find.TickManager.TicksAbs);
    }

    private static void FixRNGPos()
    {
        Rand.PopState();
    }
}