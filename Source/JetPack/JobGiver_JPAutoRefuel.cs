using RimWorld;
using Verse;
using Verse.AI;

namespace JetPack;

public class JobGiver_JPAutoRefuel : ThinkNode_JobGiver
{
    protected override Job TryGiveJob(Pawn pawn)
    {
        if (!Settings.DoAutoRefuel || !pawn.IsColonistPlayerControlled)
        {
            return null;
        }

        if (pawn.InMentalState)
        {
            return null;
        }

        if (pawn.Map == null)
        {
            return null;
        }

        var jobdef = DefDatabase<JobDef>.GetNamed("JPRefuel");
        if (pawn.CurJobDef == jobdef)
        {
            return null;
        }

        var JP = JPUtility.GetWornJP(pawn);
        if (JP == null)
        {
            return null;
        }

        var FuelMax = ((JetPackApparel)JP).JPFuelMax;
        var Fuel = (JP as JetPackApparel).JPFuelAmount;
        var FuelItem = (JP as JetPackApparel).JPFuelItem;
        if (FuelMax <= 0 || Fuel >= FuelMax || Fuel * 100 / FuelMax > Settings.RefuelPCT)
        {
            return null;
        }

        FindBestRefuel(pawn, FuelItem, FuelMax, Fuel, out var targ);
        return targ != null ? new Job(jobdef, targ) : null;
    }

    internal void FindBestRefuel(Pawn pilot, ThingDef FuelItem, int FuelMax, int Fuel, out Thing targ)
    {
        targ = null;
        if (pilot?.Map == null)
        {
            return;
        }

        var listfuel = pilot.Map.listerThings.ThingsOfDef(FuelItem);
        var fuelneeded = FuelMax - Fuel;
        if (fuelneeded > FuelItem.stackLimit)
        {
            fuelneeded = FuelItem.stackLimit;
        }

        if (listfuel.Count <= 0)
        {
            return;
        }

        Thing besttarg = null;
        var bestpoints = 0f;
        foreach (var targchk in listfuel)
        {
            if (targchk.IsForbidden(pilot) || targchk?.Faction is { IsPlayer: false } ||
                !pilot.CanReserveAndReach(targchk, PathEndMode.ClosestTouch, Danger.None))
            {
                continue;
            }

            float targpoints;
            if (targchk == null)
            {
                continue;
            }

            if (targchk.stackCount >= fuelneeded)
            {
                targpoints = targchk.stackCount / pilot.Position.DistanceTo(targchk.Position);
            }
            else
            {
                targpoints = targchk.stackCount / (pilot.Position.DistanceTo(targchk.Position) * 2f);
            }

            if (!(targpoints > bestpoints))
            {
                continue;
            }

            besttarg = targchk;
            bestpoints = targpoints;
        }

        if (besttarg != null)
        {
            targ = besttarg;
        }
    }
}