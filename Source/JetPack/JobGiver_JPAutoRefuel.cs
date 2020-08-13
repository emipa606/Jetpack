using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace JetPack
{
    // Token: 0x0200000F RID: 15
    public class JobGiver_JPAutoRefuel : ThinkNode_JobGiver
    {
        // Token: 0x0600003E RID: 62 RVA: 0x000038C0 File Offset: 0x00001AC0
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
            if ((pawn?.Map) == null)
            {
                return null;
            }
            JobDef jobdef = DefDatabase<JobDef>.GetNamed("JPRefuel", true);
            if ((pawn?.CurJobDef) == jobdef)
            {
                return null;
            }
            Apparel JP = JPUtility.GetWornJP(pawn);
            if (JP != null)
            {
                int FuelMax = (JP as JetPackApparel).JPFuelMax;
                int Fuel = (JP as JetPackApparel).JPFuelAmount;
                ThingDef FuelItem = (JP as JetPackApparel).JPFuelItem;
                if (FuelMax > 0 && Fuel < FuelMax && Fuel * 100 / FuelMax <= Settings.RefuelPCT)
                {
                    this.FindBestRefuel(pawn, FuelItem, FuelMax, Fuel, out Thing targ);
                    if (targ != null)
                    {
                        return new Job(jobdef, targ);
                    }
                }
            }
            return null;
        }

        // Token: 0x0600003F RID: 63 RVA: 0x0000397C File Offset: 0x00001B7C
        internal void FindBestRefuel(Pawn pilot, ThingDef FuelItem, int FuelMax, int Fuel, out Thing targ)
        {
            targ = null;
            if ((pilot?.Map) != null)
            {
                List<Thing> listfuel = pilot?.Map.listerThings.ThingsOfDef(FuelItem);
                int fuelneeded = FuelMax - Fuel;
                if (fuelneeded > FuelItem.stackLimit)
                {
                    fuelneeded = FuelItem.stackLimit;
                }
                if (listfuel.Count > 0)
                {
                    Thing besttarg = null;
                    float bestpoints = 0f;
                    for (int i = 0; i < listfuel.Count; i++)
                    {
                        Thing targchk = listfuel[i];
                        if (!ForbidUtility.IsForbidden(targchk, pilot) && ((targchk?.Faction) == null || targchk.Faction.IsPlayer) && ReservationUtility.CanReserveAndReach(pilot, targchk, PathEndMode.ClosestTouch, Danger.None, 1, -1, null, false))
                        {
                            float targpoints;
                            if (targchk.stackCount >= fuelneeded)
                            {
                                targpoints = (float)targchk.stackCount / IntVec3Utility.DistanceTo(pilot.Position, targchk.Position);
                            }
                            else
                            {
                                targpoints = (float)targchk.stackCount / (IntVec3Utility.DistanceTo(pilot.Position, targchk.Position) * 2f);
                            }
                            if (targpoints > bestpoints)
                            {
                                besttarg = targchk;
                                bestpoints = targpoints;
                            }
                        }
                    }
                    if (besttarg != null)
                    {
                        targ = besttarg;
                    }
                }
            }
        }
    }
}
