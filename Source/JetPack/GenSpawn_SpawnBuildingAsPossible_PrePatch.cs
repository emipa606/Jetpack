using System;
using HarmonyLib;
using RimWorld;
using Verse;

namespace JetPack
{
    // Token: 0x0200000B RID: 11
    [HarmonyPatch(typeof(GenSpawn), "SpawnBuildingAsPossible")]
    public class GenSpawn_SpawnBuildingAsPossible_PrePatch
    {
        // Token: 0x0600001B RID: 27 RVA: 0x00002528 File Offset: 0x00000728
        [HarmonyPrefix]
        [HarmonyPriority(800)]
        public static bool PreFix(Building building, Map map, bool respawningAfterLoad = false)
        {
            if (respawningAfterLoad && CorrectJPKeroseneRefiner.RFLoaded() && CorrectJPKeroseneRefiner.IsProblematicItem(building) && (building?.Faction) != Faction.OfPlayer)
            {
                GenSpawn.Spawn(DefDatabase<ThingDef>.GetNamed("Marble", false), building.Position, map, 0);
                return false;
            }
            return true;
        }
    }
}
