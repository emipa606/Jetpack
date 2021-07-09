using HarmonyLib;
using RimWorld;
using Verse;

namespace JetPack
{
    // Token: 0x02000005 RID: 5
    [HarmonyPatch(typeof(ApparelUtility), "CanWearTogether")]
    public class CanWearTogether_JPPostPatch
    {
        // Token: 0x06000009 RID: 9 RVA: 0x00002295 File Offset: 0x00000495
        [HarmonyPostfix]
        public static void PostFix(ref bool __result, ThingDef A, ThingDef B, BodyDef body)
        {
            if (__result && JPUtility.GetIsJPApparel(A) && JPUtility.GetIsJPApparel(B))
            {
                __result = false;
            }
        }
    }
}