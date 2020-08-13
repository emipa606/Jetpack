using System;
using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace JetPack
{
    // Token: 0x02000003 RID: 3
    [HarmonyPatch(typeof(PawnGraphicSet), "ResolveApparelGraphics")]
    public class ResolveApparelGraphics_JPPostPatch
    {
        // Token: 0x06000006 RID: 6 RVA: 0x00002178 File Offset: 0x00000378
        [HarmonyPostfix]
        [HarmonyPriority(0)]
        public static void PostFix(ref PawnGraphicSet __instance)
        {
            Pawn p = __instance.pawn;
            if (p.apparel.WornApparelCount > 0)
            {
                List<ApparelGraphicRecord> jps = new List<ApparelGraphicRecord>();
                List<ApparelGraphicRecord> newList = new List<ApparelGraphicRecord>();
                foreach (Apparel apparel in p.apparel.WornApparel)
                {
                    if (ApparelGraphicRecordGetter.TryGetGraphicApparel(apparel, p.story.bodyType, out ApparelGraphicRecord rec))
                    {
                        if (apparel is JetPackApparel)
                        {
                            jps.Add(rec);
                        }
                        else
                        {
                            newList.Add(rec);
                        }
                    }
                }
                if (jps.Count > 0)
                {
                    foreach (ApparelGraphicRecord apr in jps)
                    {
                        newList.Add(apr);
                    }
                }
                if (newList.Count > 0)
                {
                    __instance.apparelGraphics = newList;
                }
            }
        }
    }
}
