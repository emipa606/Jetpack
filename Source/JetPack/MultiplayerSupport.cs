using System.Reflection;
using HarmonyLib;
using Multiplayer.API;
using Verse;

namespace JetPack;

[StaticConstructorOnStartup]
internal static class MultiplayerSupport
{
    private static readonly Harmony harmony = new Harmony("rimworld.pelador.jetpack.multiplayersupport");

    static MultiplayerSupport()
    {
        if (!MP.enabled)
        {
            return;
        }

        MP.RegisterSyncMethod(typeof(JetPackApparel), "UseJetPack");
        MP.RegisterSyncMethod(typeof(JetPackApparel), "ToggleSlowBurn");
        MP.RegisterSyncMethod(typeof(JetPackApparel), "RefuelJP");
        MP.RegisterSyncMethod(typeof(JetPackApparel), "DropFuelJP");
        MP.RegisterSyncMethod(typeof(JetPackApparel), "ChangeFuelJP");
        MP.RegisterSyncMethod(typeof(JetPackApparel), "DebugUseJP");
        MethodInfo[] array =
        {
            AccessTools.Method(typeof(JPInjury), "CheckForExplosion"),
            AccessTools.Method(typeof(JPInjury), "DoJPRelatedInjury"),
            AccessTools.Method(typeof(JPInjury), "SetUpInjVars"),
            AccessTools.Method(typeof(JPSkyFaller), "JPImpact"),
            AccessTools.Method(typeof(JPSkyFaller), "JPIgnite")
        };
        foreach (var methodInfo in array)
        {
            FixRNG(methodInfo);
        }
    }

    private static void FixRNG(MethodInfo method)
    {
        harmony.Patch(method, new HarmonyMethod(typeof(MultiplayerSupport), "FixRNGPre"),
            new HarmonyMethod(typeof(MultiplayerSupport), "FixRNGPos"));
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