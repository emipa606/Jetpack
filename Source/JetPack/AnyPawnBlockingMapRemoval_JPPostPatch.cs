using System;
using HarmonyLib;
using Verse;

namespace JetPack
{
    // Token: 0x02000004 RID: 4
    [HarmonyPatch(typeof(MapPawns))]
    [HarmonyPatch("AnyPawnBlockingMapRemoval", MethodType.Getter)]
    internal static class AnyPawnBlockingMapRemoval_JPPostPatch
    {
        // Token: 0x06000008 RID: 8 RVA: 0x00002284 File Offset: 0x00000484
        [HarmonyPriority(800)]
        private static void Postfix(ref bool __result, Map ___map)
        {
            if (!__result && JPUtility.GetJPSkyFOnMap(___map))
            {
                __result = true;
            }
        }
    }
}
