using HarmonyLib;
using RimWorld;
using Verse;

namespace JetPack;

[HarmonyPatch(typeof(GenSpawn), nameof(GenSpawn.SpawnBuildingAsPossible))]
public class GenSpawn_SpawnBuildingAsPossible_PrePatch
{
    [HarmonyPrefix]
    [HarmonyPriority(800)]
    public static bool PreFix(Building building, Map map, bool respawningAfterLoad = false)
    {
        if (!respawningAfterLoad || !CorrectJPKeroseneRefiner.RFLoaded() ||
            !CorrectJPKeroseneRefiner.IsProblematicItem(building) || building?.Faction == Faction.OfPlayer)
        {
            return true;
        }

        if (building != null)
        {
            GenSpawn.Spawn(DefDatabase<ThingDef>.GetNamed("Marble", false), building.Position, map);
        }

        return false;
    }
}