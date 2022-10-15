using HarmonyLib;
using RimWorld;
using Verse;

namespace JetPack;

[HarmonyPatch(typeof(StatWorker), "GetValueUnfinalized")]
public class GetValueUnfinalized_JPPostPatch
{
    [HarmonyPostfix]
    public static void PostFix(ref float __result, StatWorker __instance, StatDef ___stat, StatRequest req)
    {
        if (!req.HasThing)
        {
            return;
        }

        var thing = req.Thing;
        if (!(thing is Pawn pawn))
        {
            return;
        }

        if (___stat == StatDefOf.MoveSpeed)
        {
            __result += JPUtility.GetSlowBurn(pawn);
        }
    }
}