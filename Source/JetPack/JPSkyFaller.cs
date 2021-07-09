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
        // Token: 0x04000024 RID: 36
        private readonly SoundDef JumpSound = DefDatabase<SoundDef>.GetNamed("JetPack", false);

        // Token: 0x04000023 RID: 35
        private bool anticipationSoundPlayed;

        // Token: 0x04000026 RID: 38
        private bool PilotRoofPunchDown;

        // Token: 0x04000025 RID: 37
        private bool PilotRoofPunchUp;

        // Token: 0x04000027 RID: 39
        private int ticksToHeadAche;

        // Token: 0x0600004A RID: 74 RVA: 0x0000417C File Offset: 0x0000237C
        public override void Tick()
        {
            innerContainer.ThingOwnerTick();
            ticksToImpact--;
            ticksToHeadAche++;
            var drawLoc = base.DrawPos;
            if (ticksToImpact % 3 == 0)
            {
                var numMotes = Math.Min(2, def.skyfaller.motesPerCell);
                for (var i = 0; i < numMotes; i++)
                {
                    FleckMaker.ThrowSmoke(drawLoc, Map, 2f);
                }
            }

            if (ticksToImpact % 25 == 0 && JumpSound != null)
            {
                var SoundPos = drawLoc.ToIntVec3();
                JumpSound.PlayOneShot(new TargetInfo(SoundPos, Map));
            }

            if (ticksToHeadAche == 3 && Settings.AllowFire)
            {
                JPIgnite(drawLoc.ToIntVec3(), Map);
            }

            if (ticksToHeadAche == 10)
            {
                JPHitRoof(true);
            }

            if (ticksToImpact == 15)
            {
                JPHitRoof(false);
            }

            if (!anticipationSoundPlayed && def.skyfaller.anticipationSound != null &&
                ticksToImpact < def.skyfaller.anticipationSoundTicks)
            {
                anticipationSoundPlayed = true;
                def.skyfaller.anticipationSound.PlayOneShot(new TargetInfo(Position, Map));
            }

            if (ticksToImpact == 3)
            {
                EjectPilot();
            }

            if (ticksToImpact == 0)
            {
                JPImpact();
                return;
            }

            if (ticksToImpact >= 0)
            {
                return;
            }

            Log.Error("ticksToImpact < 0. Was there an exception? Destroying skyfaller.");
            EjectPilot();
            Destroy();
        }

        // Token: 0x0600004B RID: 75 RVA: 0x00004318 File Offset: 0x00002518
        private void EjectPilot()
        {
            var pilot = GetThingForGraphic();
            if (pilot != null)
            {
                GenPlace.TryPlaceThing(pilot, Position, Map, ThingPlaceMode.Near, delegate(Thing thing, int _)
                {
                    PawnUtility.RecoverFromUnwalkablePositionOrKill(thing.Position, thing.Map);
                    if (thing.def.Fillage == FillCategory.Full && def.skyfaller.CausesExplosion &&
                        thing.Position.InHorDistOf(Position, def.skyfaller.explosionRadius))
                    {
                        Map.terrainGrid.Notify_TerrainDestroyed(thing.Position);
                    }

                    CheckDrafting(thing);
                    JPInjury.CheckDFA(thing, Position);
                    CheckAndApplyPostInjury(thing);
                    if (Settings.CooldownTime > 0)
                    {
                        JPApplyCooldown(thing, Math.Min(10, Settings.CooldownTime));
                    }
                });
            }
        }

        // Token: 0x0600004C RID: 76 RVA: 0x0000435C File Offset: 0x0000255C
        internal void JPApplyCooldown(Thing pilot, int cd)
        {
            var JP = JPUtility.GetWornJP(pilot);
            if (JP is JetPackApparel apparel)
            {
                apparel.JPCooldownTicks = Math.Max(0, 60 * cd);
            }
        }

        // Token: 0x0600004D RID: 77 RVA: 0x00004390 File Offset: 0x00002590
        internal void CheckAndApplyPostInjury(Thing pilot)
        {
            if (PilotRoofPunchUp)
            {
                JPInjury.DoJPRelatedInjury(pilot, true, false, null, false);
                PilotRoofPunchUp = false;
            }

            if (!PilotRoofPunchDown)
            {
                return;
            }

            JPInjury.DoJPRelatedInjury(pilot, false, false, null, true);
            PilotRoofPunchDown = false;
        }

        // Token: 0x0600004E RID: 78 RVA: 0x000043C4 File Offset: 0x000025C4
        internal void CheckDrafting(Thing pilot)
        {
            if (pilot is not Pawn)
            {
                return;
            }

            var JP = JPUtility.GetWornJP(pilot);
            if (JP == null || !((JetPackApparel) JP).JPPilotIsDrafted)
            {
                return;
            }

            (JP as JetPackApparel).JPPilotIsDrafted = false;
            ((Pawn) pilot).drafter.Drafted = true;
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
            if (!innerContainer.Any || innerContainer.Count <= 0)
            {
                return null;
            }

            foreach (var thingchk in innerContainer)
            {
                if (thingchk is Pawn)
                {
                    pilot = thingchk;
                }
            }

            return pilot as Pawn;
        }

        // Token: 0x06000051 RID: 81 RVA: 0x00004498 File Offset: 0x00002698
        internal void JPHitRoof(bool up)
        {
            if (!def.skyfaller.hitRoof)
            {
                return;
            }

            CellRect cr;
            if (up)
            {
                var hrpcell = base.DrawPos.ToIntVec3();
                var punchsize = new IntVec2(3, 3);
                cr = GenAdj.OccupiedRect(hrpcell, Rotation, punchsize);
            }
            else
            {
                cr = this.OccupiedRect();
            }

            if (!cr.Cells.Any(x => x.Roofed(Map)))
            {
                return;
            }

            var roof = cr.Cells.First(x => x.Roofed(Map)).GetRoof(Map);
            if (!roof.soundPunchThrough.NullOrUndefined())
            {
                roof.soundPunchThrough.PlayOneShot(new TargetInfo(Position, Map));
            }

            RoofCollapserImmediate.DropRoofInCells(cr.ExpandedBy(1).ClipInsideMap(Map).Cells.Where(
                delegate(IntVec3 c)
                {
                    if (!c.InBounds(Map))
                    {
                        return false;
                    }

                    if (cr.Contains(c))
                    {
                        return true;
                    }

                    if (c.GetFirstPawn(Map) != null)
                    {
                        return false;
                    }

                    var edifice = c.GetEdifice(Map);
                    return edifice == null || !edifice.def.holdsRoof;
                }), Map);
            if (up)
            {
                PilotRoofPunchUp = true;
                return;
            }

            PilotRoofPunchDown = true;
        }

        // Token: 0x06000052 RID: 82 RVA: 0x000045CC File Offset: 0x000027CC
        public void JPImpact()
        {
            if (def.skyfaller.CausesExplosion)
            {
                GenExplosion.DoExplosion(Position, Map, def.skyfaller.explosionRadius, def.skyfaller.explosionDamage,
                    null,
                    GenMath.RoundRandom(def.skyfaller.explosionDamage.defaultDamage *
                                        def.skyfaller.explosionDamageFactor));
            }

            for (var num = innerContainer.Count - 1; num >= 0; num--)
            {
                GenPlace.TryPlaceThing(innerContainer[num], Position, Map, ThingPlaceMode.Near,
                    delegate(Thing thing, int _)
                    {
                        PawnUtility.RecoverFromUnwalkablePositionOrKill(thing.Position, thing.Map);
                        if (thing.def.Fillage == FillCategory.Full && def.skyfaller.CausesExplosion &&
                            thing.Position.InHorDistOf(Position, def.skyfaller.explosionRadius))
                        {
                            Map.terrainGrid.Notify_TerrainDestroyed(thing.Position);
                        }
                    });
            }

            innerContainer.ClearAndDestroyContents();
            var cellRect = this.OccupiedRect();
            for (var i = 0; i < cellRect.Area * def.skyfaller.motesPerCell; i++)
            {
                FleckMaker.ThrowDustPuff(cellRect.RandomVector3, Map, 2f);
            }

            if (def.skyfaller.MakesShrapnel)
            {
                SkyfallerShrapnelUtility.MakeShrapnel(Position, Map, shrapnelDirection,
                    def.skyfaller.shrapnelDistanceFactor, def.skyfaller.metalShrapnelCountRange.RandomInRange,
                    def.skyfaller.rubbleShrapnelCountRange.RandomInRange, true);
            }

            if (def.skyfaller.cameraShake > 0f && Map == Find.CurrentMap)
            {
                Find.CameraDriver.shaker.DoShake(def.skyfaller.cameraShake);
            }

            def.skyfaller.impactSound?.PlayOneShot(SoundInfo.InMap(new TargetInfo(Position, Map)));

            Destroy();
        }

        // Token: 0x06000053 RID: 83 RVA: 0x00004800 File Offset: 0x00002A00
        public void JPIgnite(IntVec3 cell, Map map)
        {
            var pilot = GetThingForGraphic();
            if (pilot == null)
            {
                return;
            }

            var JP = JPUtility.GetWornJP(pilot);
            if (JP == null)
            {
                return;
            }

            var jetPackApparel = JP as JetPackApparel;
            var fuel = jetPackApparel?.JPFuelItem;
            if (fuel == null)
            {
                return;
            }

            var factor = JPUtility.GetIgnitionFactor(fuel);
            var Rnd = Rand.Range(1, 100);
            if (Rnd < factor)
            {
                FireUtility.TryStartFireIn(cell, map, Math.Max(10f, factor - Rnd) / 100f);
            }
        }
    }
}