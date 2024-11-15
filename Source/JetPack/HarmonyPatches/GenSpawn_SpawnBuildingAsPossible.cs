using HarmonyLib;
using RimWorld;
using Verse;

namespace JetPack.HarmonyPatches;

[HarmonyPatch(typeof(GenSpawn), nameof(GenSpawn.SpawnBuildingAsPossible))]
public class GenSpawn_SpawnBuildingAsPossible
{
    [HarmonyPriority(800)]
    public static bool Prefix(Building building, Map map, bool respawningAfterLoad = false)
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