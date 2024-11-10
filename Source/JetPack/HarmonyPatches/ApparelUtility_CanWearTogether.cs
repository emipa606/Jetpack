using HarmonyLib;
using RimWorld;
using Verse;

namespace JetPack.HarmonyPatches;

[HarmonyPatch(typeof(ApparelUtility), nameof(ApparelUtility.CanWearTogether))]
public class ApparelUtility_CanWearTogether
{
    public static void Postfix(ref bool __result, ThingDef A, ThingDef B)
    {
        if (__result && JPUtility.GetIsJPApparel(A) && JPUtility.GetIsJPApparel(B))
        {
            __result = false;
        }
    }
}