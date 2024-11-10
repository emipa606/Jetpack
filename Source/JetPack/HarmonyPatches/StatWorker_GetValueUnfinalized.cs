using HarmonyLib;
using RimWorld;
using Verse;

namespace JetPack.HarmonyPatches;

[HarmonyPatch(typeof(StatWorker), nameof(StatWorker.GetValueUnfinalized))]
public class StatWorker_GetValueUnfinalized
{
    public static void Postfix(ref float __result, StatDef ___stat, StatRequest req)
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