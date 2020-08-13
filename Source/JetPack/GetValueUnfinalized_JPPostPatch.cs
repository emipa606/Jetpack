using System;
using HarmonyLib;
using RimWorld;
using Verse;

namespace JetPack
{
    // Token: 0x0200000C RID: 12
    [HarmonyPatch(typeof(StatWorker), "GetValueUnfinalized")]
    public class GetValueUnfinalized_JPPostPatch
    {
        // Token: 0x0600001D RID: 29 RVA: 0x00002580 File Offset: 0x00000780
        [HarmonyPostfix]
        public static void PostFix(ref float __result, StatWorker __instance, StatDef ___stat, StatRequest req)
        {
            if (!req.HasThing)
            {
                return;
            }
            Thing thing = req.Thing;
            if (!(thing is Pawn))
            {
                return;
            }
            if (___stat == StatDefOf.MoveSpeed)
            {
                __result += JPUtility.GetSlowBurn(thing as Pawn);
            }
        }
    }
}
