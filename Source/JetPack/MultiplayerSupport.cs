using System.Reflection;
using HarmonyLib;
using Multiplayer.API;
using Verse;

namespace JetPack
{
    // Token: 0x02000015 RID: 21
    [StaticConstructorOnStartup]
    internal static class MultiplayerSupport
    {
        // Token: 0x04000035 RID: 53
        private static readonly Harmony harmony = new Harmony("rimworld.pelador.jetpack.multiplayersupport");

        // Token: 0x06000074 RID: 116 RVA: 0x0000552C File Offset: 0x0000372C
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

        // Token: 0x06000075 RID: 117 RVA: 0x0000566E File Offset: 0x0000386E
        private static void FixRNG(MethodInfo method)
        {
            harmony.Patch(method, new HarmonyMethod(typeof(MultiplayerSupport), "FixRNGPre"),
                new HarmonyMethod(typeof(MultiplayerSupport), "FixRNGPos"));
        }

        // Token: 0x06000076 RID: 118 RVA: 0x000056A8 File Offset: 0x000038A8
        private static void FixRNGPre()
        {
            Rand.PushState(Find.TickManager.TicksAbs);
        }

        // Token: 0x06000077 RID: 119 RVA: 0x000056B9 File Offset: 0x000038B9
        private static void FixRNGPos()
        {
            Rand.PopState();
        }
    }
}