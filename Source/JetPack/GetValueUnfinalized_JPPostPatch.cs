using HarmonyLib;
using RimWorld;
using Verse;

namespace JetPack;

[HarmonyPatch(typeof(StatWorker), nameof(StatWorker.GetValueUnfinalized))]
public class GetValueUnfinalized_JPPostPatch
{
    [HarmonyPostfix]
    public static void PostFix(ref float __result, StatDef ___stat, StatRequest req)
    {
        if (!req.HasThing)
        {
            return;
        }

        var thing = req.Thing;
        if (thing is not Pawn pawn)
        {
            return;
        }

        if (___stat == StatDefOf.MoveSpeed)
        {
            __result += JPUtility.GetSlowBurn(pawn);
        }
    }
}