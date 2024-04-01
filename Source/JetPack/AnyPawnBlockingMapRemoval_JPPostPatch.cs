using HarmonyLib;
using Verse;

namespace JetPack;

[HarmonyPatch(typeof(MapPawns), nameof(MapPawns.AnyPawnBlockingMapRemoval), MethodType.Getter)]
internal static class AnyPawnBlockingMapRemoval_JPPostPatch
{
    [HarmonyPriority(800)]
    private static void Postfix(ref bool __result, Map ___map)
    {
        if (!__result && JPUtility.GetJPSkyFOnMap(___map))
        {
            __result = true;
        }
    }
}