using System;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace JetPack
{
    // Token: 0x02000012 RID: 18
    public class JPSkyFaller : Skyfaller
    {
        // Token: 0x0600004A RID: 74 RVA: 0x0000417C File Offset: 0x0000237C
        public override void Tick()
        {
            this.innerContainer.ThingOwnerTick(true);
            this.ticksToImpact--;
            this.ticksToHeadAche++;
            Vector3 drawLoc = base.DrawPos;
            if (this.ticksToImpact % 3 == 0)
            {
                int numMotes = Math.Min(2, this.def.skyfaller.motesPerCell);
                for (int i = 0; i < numMotes; i++)
                {
                    MoteMaker.ThrowSmoke(drawLoc, base.Map, 2f);
                }
            }
            if (this.ticksToImpact % 25 == 0 && this.JumpSound != null)
            {
                IntVec3 SoundPos = IntVec3Utility.ToIntVec3(drawLoc);
                SoundStarter.PlayOneShot(this.JumpSound, new TargetInfo(SoundPos, base.Map, false));
            }
            if (this.ticksToHeadAche == 3 && Settings.AllowFire)
            {
                this.JPIgnite(IntVec3Utility.ToIntVec3(drawLoc), base.Map);
            }
            if (this.ticksToHeadAche == 10)
            {
                this.JPHitRoof(true);
            }
            if (this.ticksToImpact == 15)
            {
                this.JPHitRoof(false);
            }
            if (!this.anticipationSoundPlayed && this.def.skyfaller.anticipationSound != null && this.ticksToImpact < this.def.skyfaller.anticipationSoundTicks)
            {
                this.anticipationSoundPlayed = true;
                SoundStarter.PlayOneShot(this.def.skyfaller.anticipationSound, new TargetInfo(base.Position, base.Map, false));
            }
            if (this.ticksToImpact == 3)
            {
                this.EjectPilot();
            }
            if (this.ticksToImpact == 0)
            {
                this.JPImpact();
                return;
            }
            if (this.ticksToImpact < 0)
            {
                Log.Error("ticksToImpact < 0. Was there an exception? Destroying skyfaller.", false);
                this.EjectPilot();
                this.Destroy(0);
            }
        }

        // Token: 0x0600004B RID: 75 RVA: 0x00004318 File Offset: 0x00002518
        private void EjectPilot()
        {
            Thing pilot = this.GetThingForGraphic();
            if (pilot != null)
            {
                GenPlace.TryPlaceThing(pilot, base.Position, base.Map, ThingPlaceMode.Near, delegate (Thing thing, int count)
                {
                    PawnUtility.RecoverFromUnwalkablePositionOrKill(thing.Position, thing.Map);
                    if (thing.def.Fillage == FillCategory.Full && this.def.skyfaller.CausesExplosion && thing.Position.InHorDistOf(base.Position, this.def.skyfaller.explosionRadius))
                    {
                        base.Map.terrainGrid.Notify_TerrainDestroyed(thing.Position);
                    }
                    this.CheckDrafting(thing);
                    JPInjury.CheckDFA(thing, base.Position);
                    this.CheckAndApplyPostInjury(thing);
                    if (Settings.CooldownTime > 0)
                    {
                        this.JPApplyCooldown(thing, Math.Min(10, Settings.CooldownTime));
                    }
                }, null, default);
            }
        }

        // Token: 0x0600004C RID: 76 RVA: 0x0000435C File Offset: 0x0000255C
        internal void JPApplyCooldown(Thing pilot, int cd)
        {
            Apparel JP = JPUtility.GetWornJP(pilot);
            if (JP != null && JP is JetPackApparel)
            {
                (JP as JetPackApparel).JPCooldownTicks = Math.Max(0, 60 * cd);
            }
        }

        // Token: 0x0600004D RID: 77 RVA: 0x00004390 File Offset: 0x00002590
        internal void CheckAndApplyPostInjury(Thing pilot)
        {
            if (this.PilotRoofPunchUp)
            {
                JPInjury.DoJPRelatedInjury(pilot, true, false, null, false);
                this.PilotRoofPunchUp = false;
            }
            if (this.PilotRoofPunchDown)
            {
                JPInjury.DoJPRelatedInjury(pilot, false, false, null, true);
                this.PilotRoofPunchDown = false;
            }
        }

        // Token: 0x0600004E RID: 78 RVA: 0x000043C4 File Offset: 0x000025C4
        internal void CheckDrafting(Thing pilot)
        {
            if (pilot != null && pilot is Pawn)
            {
                Apparel JP = JPUtility.GetWornJP(pilot);
                if (JP != null && (JP as JetPackApparel).JPPilotIsDrafted)
                {
                    (JP as JetPackApparel).JPPilotIsDrafted = false;
                    (pilot as Pawn).drafter.Drafted = true;
                }
            }
        }

        // Token: 0x0600004F RID: 79 RVA: 0x00004410 File Offset: 0x00002610
        public override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            if (GetThingForGraphic() is Pawn Pilot)
            {
                new PawnRenderer(Pilot).RenderPawnAt(drawLoc);
            }
        }

        // Token: 0x06000050 RID: 80 RVA: 0x00004438 File Offset: 0x00002638
        private Thing GetThingForGraphic()
        {
            Thing pilot = null;
            if (this.innerContainer.Any && this.innerContainer.Count > 0)
            {
                for (int i = 0; i < this.innerContainer.Count; i++)
                {
                    Thing thingchk = this.innerContainer[i];
                    if (thingchk is Pawn)
                    {
                        pilot = thingchk;
                    }
                }
            }
            return pilot as Pawn;
        }

        // Token: 0x06000051 RID: 81 RVA: 0x00004498 File Offset: 0x00002698
        internal void JPHitRoof(bool up)
        {
            if (!this.def.skyfaller.hitRoof)
            {
                return;
            }
            CellRect cr;
            if (up)
            {
                IntVec3 hrpcell = IntVec3Utility.ToIntVec3(base.DrawPos);
                IntVec2 punchsize = new IntVec2(3, 3);
                cr = GenAdj.OccupiedRect(hrpcell, base.Rotation, punchsize);
            }
            else
            {
                cr = GenAdj.OccupiedRect(this);
            }
            if (cr.Cells.Any((IntVec3 x) => GridsUtility.Roofed(x, this.Map)))
            {
                RoofDef roof = GridsUtility.GetRoof(cr.Cells.First((IntVec3 x) => GridsUtility.Roofed(x, this.Map)), base.Map);
                if (!SoundDefHelper.NullOrUndefined(roof.soundPunchThrough))
                {
                    SoundStarter.PlayOneShot(roof.soundPunchThrough, new TargetInfo(base.Position, base.Map, false));
                }
                RoofCollapserImmediate.DropRoofInCells(cr.ExpandedBy(1).ClipInsideMap(base.Map).Cells.Where(delegate (IntVec3 c)
                {
                    if (!GenGrid.InBounds(c, this.Map))
                    {
                        return false;
                    }
                    if (cr.Contains(c))
                    {
                        return true;
                    }
                    if (GridsUtility.GetFirstPawn(c, this.Map) != null)
                    {
                        return false;
                    }
                    Building edifice = GridsUtility.GetEdifice(c, this.Map);
                    return edifice == null || !edifice.def.holdsRoof;
                }), base.Map, null);
                if (up)
                {
                    this.PilotRoofPunchUp = true;
                    return;
                }
                this.PilotRoofPunchDown = true;
            }
        }

        // Token: 0x06000052 RID: 82 RVA: 0x000045CC File Offset: 0x000027CC
        public void JPImpact()
        {
            if (this.def.skyfaller.CausesExplosion)
            {
                GenExplosion.DoExplosion(base.Position, base.Map, this.def.skyfaller.explosionRadius, this.def.skyfaller.explosionDamage, null, GenMath.RoundRandom((float)this.def.skyfaller.explosionDamage.defaultDamage * this.def.skyfaller.explosionDamageFactor), -1f, null, null, null, null, null, 0f, 1, false, null, 0f, 1, 0f, false, null, null);
            }
            for (int num = this.innerContainer.Count - 1; num >= 0; num--)
            {
                GenPlace.TryPlaceThing(this.innerContainer[num], base.Position, base.Map, ThingPlaceMode.Near, delegate (Thing thing, int count)
                {
                    PawnUtility.RecoverFromUnwalkablePositionOrKill(thing.Position, thing.Map);
                    if (thing.def.Fillage == FillCategory.Full && this.def.skyfaller.CausesExplosion && thing.Position.InHorDistOf(base.Position, this.def.skyfaller.explosionRadius))
                    {
                        base.Map.terrainGrid.Notify_TerrainDestroyed(thing.Position);
                    }
                }, null, default);
            }
            this.innerContainer.ClearAndDestroyContents(0);
            CellRect cellRect = GenAdj.OccupiedRect(this);
            for (int i = 0; i < cellRect.Area * this.def.skyfaller.motesPerCell; i++)
            {
                MoteMaker.ThrowDustPuff(cellRect.RandomVector3, base.Map, 2f);
            }
            if (this.def.skyfaller.MakesShrapnel)
            {
                SkyfallerShrapnelUtility.MakeShrapnel(base.Position, base.Map, this.shrapnelDirection, this.def.skyfaller.shrapnelDistanceFactor, this.def.skyfaller.metalShrapnelCountRange.RandomInRange, this.def.skyfaller.rubbleShrapnelCountRange.RandomInRange, true);
            }
            if (this.def.skyfaller.cameraShake > 0f && base.Map == Find.CurrentMap)
            {
                Find.CameraDriver.shaker.DoShake(this.def.skyfaller.cameraShake);
            }
            if (this.def.skyfaller.impactSound != null)
            {
                SoundStarter.PlayOneShot(this.def.skyfaller.impactSound, SoundInfo.InMap(new TargetInfo(base.Position, base.Map, false), 0));
            }
            this.Destroy(0);
        }

        // Token: 0x06000053 RID: 83 RVA: 0x00004800 File Offset: 0x00002A00
        public void JPIgnite(IntVec3 cell, Map map)
        {
            Thing pilot = this.GetThingForGraphic();
            if (pilot != null)
            {
                Apparel JP = JPUtility.GetWornJP(pilot);
                if (JP != null)
                {
                    JetPackApparel jetPackApparel = JP as JetPackApparel;
                    ThingDef fuel = jetPackApparel?.JPFuelItem;
                    if (fuel != null)
                    {
                        float factor = JPUtility.GetIgnitionFactor(fuel);
                        int Rnd = Rand.Range(1, 100);
                        if ((float)Rnd < factor)
                        {
                            FireUtility.TryStartFireIn(cell, map, Math.Max(10f, factor - (float)Rnd) / 100f);
                        }
                    }
                }
            }
        }

        // Token: 0x04000023 RID: 35
        private bool anticipationSoundPlayed;

        // Token: 0x04000024 RID: 36
        private readonly SoundDef JumpSound = DefDatabase<SoundDef>.GetNamed("JetPack", false);

        // Token: 0x04000025 RID: 37
        private bool PilotRoofPunchUp;

        // Token: 0x04000026 RID: 38
        private bool PilotRoofPunchDown;

        // Token: 0x04000027 RID: 39
        private int ticksToHeadAche;
    }
}
