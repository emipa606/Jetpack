using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace JetPack
{
    // Token: 0x02000011 RID: 17
    public class JPInjury
    {
        // Token: 0x04000022 RID: 34
        internal const float PEP = 0.33f;

        // Token: 0x06000042 RID: 66 RVA: 0x00003AC8 File Offset: 0x00001CC8
        public static bool CheckForExplosion(JetPackApparel JP)
        {
            if (!Settings.AllowBoom)
            {
                return false;
            }

            var hp = JP.HitPoints;
            var maxhp = JP.MaxHitPoints;
            if (!(hp < maxhp * 0.33f))
            {
                return false;
            }

            var chance = ((maxhp * 0.33f) - hp) * 100f / maxhp;
            if (Rand.Range(1, 100) < chance)
            {
                return true;
            }

            return false;
        }

        // Token: 0x06000043 RID: 67 RVA: 0x00003B1C File Offset: 0x00001D1C
        internal static void DoJPExplosion(Pawn pilot, int fuel, ThingDef fueldef)
        {
            if (fueldef.GetCompProperties<CompProperties_Explosive>() == null)
            {
                return;
            }

            var compfueldef = fueldef.GetCompProperties<CompProperties_Explosive>();
            var radius = compfueldef.explosiveRadius + (compfueldef.explosiveExpandPerStackcount * fuel);
            var dmgdef = compfueldef.explosiveDamageType;
            var preThingDef = compfueldef.preExplosionSpawnThingDef;
            var dmg = (int) (fuel * GetBoomFactor(fueldef));
            GenExplosion.DoExplosion(pilot.Position, pilot.Map, radius, dmgdef, pilot, dmg, -1f, null, null, null,
                null, null, 1f, 2, false, preThingDef, 1f, 1, 0.9f, true);
            FleckMaker.ThrowSmoke(pilot.Position.ToVector3(), pilot.Map, 1f);
        }

        // Token: 0x06000044 RID: 68 RVA: 0x00003BAC File Offset: 0x00001DAC
        internal static float GetBoomFactor(ThingDef fueldef)
        {
            var factor = 0.15f;
            var defName = fueldef.defName;
            if (defName != "MSHydrogenPeroxide")
            {
                if (defName == "JPKerosene")
                {
                    factor *= 1.1f;
                }
            }
            else
            {
                factor *= 0.9f;
            }

            return factor;
        }

        // Token: 0x06000045 RID: 69 RVA: 0x00003BF8 File Offset: 0x00001DF8
        internal static void CheckDFA(Thing instigator, IntVec3 DFACell)
        {
            if (!Settings.ApplyDFA || instigator?.Map == null)
            {
                return;
            }

            var DFA = false;
            var injuryFull = false;
            if (DFAEvent(DFACell, instigator, true))
            {
                DFA = true;
                injuryFull = true;
            }

            if (Settings.ApplyDFASplash)
            {
                var neighbours = GenRadial.RadialCellsAround(DFACell, 1.5f, false).ToList();
                if (neighbours.Count > 0)
                {
                    foreach (var neighbour in neighbours)
                    {
                        if (neighbour.InBounds(instigator.Map) && DFAEvent(neighbour, instigator, false))
                        {
                            DFA = true;
                        }
                    }
                }
            }

            if (DFA)
            {
                DoJPRelatedInjury(instigator, false, true, null, injuryFull);
            }
        }

        // Token: 0x06000046 RID: 70 RVA: 0x00003CC0 File Offset: 0x00001EC0
        internal static bool DFAEvent(IntVec3 cell, Thing instigator, bool fullDmg)
        {
            var DFAEvent = false;
            var cellThings = cell.GetThingList(instigator.Map);
            if (cellThings.Count <= 0)
            {
                return false;
            }

            foreach (var victim in cellThings)
            {
                if (victim is not Pawn || victim == instigator)
                {
                    continue;
                }

                DoJPRelatedInjury(victim, true, true, instigator, fullDmg);
                DFAEvent = true;
            }

            return DFAEvent;
        }

        // Token: 0x06000047 RID: 71 RVA: 0x00003D24 File Offset: 0x00001F24
        public static void DoJPRelatedInjury(Thing victim, bool headSpace, bool isDFA, Thing instigator, bool fullDmg)
        {
            if (victim is not Pawn)
            {
                return;
            }

            SetUpInjVars((Pawn) victim, headSpace, isDFA, instigator, out var candidate, out var injChance,
                out var DamDef, out var dmg);
            if (candidate == null || !(Rand.Range(1f, 100f) <= injChance))
            {
                return;
            }

            var armPen = 0f;
            if (!fullDmg)
            {
                dmg = Math.Max(1f, dmg * (Settings.DFASplashFactor / 100f));
            }

            var dinfo = new DamageInfo(DamDef, dmg, armPen, -1f, instigator, candidate);
            ((Pawn) victim).TakeDamage(dinfo);
        }

        // Token: 0x06000048 RID: 72 RVA: 0x00003DAC File Offset: 0x00001FAC
        public static void SetUpInjVars(Pawn Victim, bool headspace, bool dfa, Thing instigator,
            out BodyPartRecord candidate, out float injuryChance, out DamageDef DamDef, out float dmg)
        {
            DamDef = null;
            var Rnd = Rand.Range(1, 100);
            switch (Rnd)
            {
                case < 50:
                    DamDef = DamageDefOf.Crush;
                    break;
                case >= 50 and < 75:
                    DamDef = DamageDefOf.Blunt;
                    break;
                case >= 75 and < 87:
                    DamDef = DamageDefOf.Stab;
                    break;
                default:
                    DamDef = DamageDefOf.Stun;
                    break;
            }

            dmg = 1f;
            if (dfa && instigator != null)
            {
                dmg = Rand.Range(5f, 10f);
                if (headspace)
                {
                    dmg *= 2f;
                }
            }
            else
            {
                dmg = Rand.Range(3f, 8f);
            }

            if (instigator is Pawn pawn1)
            {
                var bodyI = pawn1.BodySize;
                if (bodyI > 1.5f)
                {
                    bodyI = 1.5f;
                }

                if (bodyI < 0.75f)
                {
                    bodyI = 0.75f;
                }

                dmg *= bodyI;
            }

            if (Victim != null)
            {
                var bodyV = Victim.BodySize;
                if (bodyV > 0f)
                {
                    dmg /= bodyV;
                }
            }

            if (DamDef == DamageDefOf.Stun)
            {
                dmg *= 2f;
            }

            candidate = null;
            var potentials = new List<BodyPartRecord>();
            if (headspace)
            {
                var body = Victim?.RaceProps.body;
                if (body?.GetPartsWithDef(BodyPartDefOf.Head) != null)
                {
                    potentials.AddRange(Victim.RaceProps.body.GetPartsWithDef(BodyPartDefOf.Head));
                }

                var body2 = Victim?.RaceProps.body;
                if (body2?.GetPartsWithDef(BodyPartDefOf.Neck) != null)
                {
                    potentials.AddRange(Victim.RaceProps.body.GetPartsWithDef(BodyPartDefOf.Neck));
                }

                var body3 = Victim?.RaceProps.body;
                if (body3?.GetPartsWithDef(BodyPartDefOf.Arm) != null)
                {
                    potentials.AddRange(Victim.RaceProps.body.GetPartsWithDef(BodyPartDefOf.Arm));
                }

                var body4 = Victim?.RaceProps.body;
                if (body4?.GetPartsWithDef(BodyPartDefOf.Torso) != null)
                {
                    potentials.AddRange(Victim.RaceProps.body.GetPartsWithDef(BodyPartDefOf.Torso));
                }

                var body5 = Victim?.RaceProps.body;
                if (body5?.GetPartsWithDef(BodyPartDefOf.InsectHead) != null)
                {
                    potentials.AddRange(Victim.RaceProps.body.GetPartsWithDef(BodyPartDefOf.InsectHead));
                }
            }
            else
            {
                var body6 = Victim?.RaceProps.body;
                if (body6?.GetPartsWithDef(BodyPartDefOf.Leg) != null)
                {
                    potentials.AddRange(Victim.RaceProps.body.GetPartsWithDef(BodyPartDefOf.Leg));
                }
            }

            if (potentials.Count <= 0)
            {
                var body7 = Victim?.RaceProps.body;
                if (body7?.GetPartsWithDef(BodyPartDefOf.Body) != null)
                {
                    potentials.AddRange(Victim.RaceProps.body.GetPartsWithDef(BodyPartDefOf.Body));
                }
            }

            if (potentials.Count > 0)
            {
                candidate = potentials.RandomElement();
            }

            injuryChance = 50f;
            if (dfa && headspace)
            {
                injuryChance = 75f;
            }

            if (instigator is not Pawn)
            {
                return;
            }

            var pawn = (Pawn) instigator;
            if (pawn.skills.GetSkill(SkillDefOf.Melee) != null)
            {
                var MeleeSkill = ((Pawn) instigator).skills.GetSkill(SkillDefOf.Melee).levelInt;
                injuryChance += (MeleeSkill - 8f) / 2f;
                dmg = Math.Max(dmg, dmg + ((MeleeSkill - 8f) / 3f));
            }

            var pawn2 = (Pawn) instigator;
            if (pawn2.skills.GetSkill(SkillDefOf.Shooting) == null)
            {
                return;
            }

            var ShootingSkill = ((Pawn) instigator).skills.GetSkill(SkillDefOf.Shooting).levelInt;
            injuryChance += (ShootingSkill - 10f) / 1.5f;
            dmg = Math.Max(dmg, dmg + ((ShootingSkill - 10f) / 3f));
        }
    }
}