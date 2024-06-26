using HarmonyLib;
using RimWorld;
using Verse;

namespace JetPack;

[HarmonyPatch(typeof(ApparelUtility), nameof(ApparelUtility.CanWearTogether))]
public class CanWearTogether_JPPostPatch
{
    [HarmonyPostfix]
    public static void PostFix(ref bool __result, ThingDef A, ThingDef B)
    {
        if (__result && JPUtility.GetIsJPApparel(A) && JPUtility.GetIsJPApparel(B))
        {
            __result = false;
        }
    }
}