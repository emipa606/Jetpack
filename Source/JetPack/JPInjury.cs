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
        // Token: 0x06000042 RID: 66 RVA: 0x00003AC8 File Offset: 0x00001CC8
        public static bool CheckForExplosion(JetPackApparel JP)
        {
            if (Settings.AllowBoom)
            {
                int hp = JP.HitPoints;
                int maxhp = JP.MaxHitPoints;
                if ((float)hp < (float)maxhp * 0.33f)
                {
                    float chance = ((float)maxhp * 0.33f - (float)hp) * 100f / (float)maxhp;
                    if ((float)Rand.Range(1, 100) < chance)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        // Token: 0x06000043 RID: 67 RVA: 0x00003B1C File Offset: 0x00001D1C
        internal static void DoJPExplosion(Pawn pilot, int fuel, ThingDef fueldef)
        {
            if (fueldef.GetCompProperties<CompProperties_Explosive>() != null)
            {
                CompProperties_Explosive compfueldef = fueldef.GetCompProperties<CompProperties_Explosive>();
                float radius = compfueldef.explosiveRadius + compfueldef.explosiveExpandPerStackcount * (float)fuel;
                DamageDef dmgdef = compfueldef.explosiveDamageType;
                ThingDef preThingDef = compfueldef.preExplosionSpawnThingDef;
                ThingDef postThingDef = ThingDefOf.Mote_Smoke;
                int dmg = (int)((float)fuel * JPInjury.GetBoomFactor(fueldef));
                GenExplosion.DoExplosion(pilot.Position, pilot.Map, radius, dmgdef, pilot, dmg, -1f, null, null, null, null, postThingDef, 1f, 2, false, preThingDef, 1f, 1, 0.9f, true, null, null);
            }
        }

        // Token: 0x06000044 RID: 68 RVA: 0x00003BAC File Offset: 0x00001DAC
        internal static float GetBoomFactor(ThingDef fueldef)
        {
            float factor = 0.15f;
            string defName = fueldef.defName;
            if (!(defName == "MSHydrogenPeroxide"))
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
            if (Settings.ApplyDFA && instigator != null && (instigator?.Map) != null)
            {
                bool DFA = false;
                bool injuryFull = false;
                if (JPInjury.DFAEvent(DFACell, instigator, true))
                {
                    DFA = true;
                    injuryFull = true;
                }
                if (Settings.ApplyDFASplash)
                {
                    List<IntVec3> neighbours = GenRadial.RadialCellsAround(DFACell, 1.5f, false).ToList<IntVec3>();
                    if (neighbours.Count > 0)
                    {
                        foreach (IntVec3 neighbour in neighbours)
                        {
                            if (GenGrid.InBounds(neighbour, instigator.Map) && JPInjury.DFAEvent(neighbour, instigator, false))
                            {
                                DFA = true;
                            }
                        }
                    }
                }
                if (DFA)
                {
                    JPInjury.DoJPRelatedInjury(instigator, false, true, null, injuryFull);
                }
            }
        }

        // Token: 0x06000046 RID: 70 RVA: 0x00003CC0 File Offset: 0x00001EC0
        internal static bool DFAEvent(IntVec3 cell, Thing instigator, bool fullDmg)
        {
            bool DFAEvent = false;
            List<Thing> cellThings = GridsUtility.GetThingList(cell, instigator.Map);
            if (cellThings.Count > 0)
            {
                for (int i = 0; i < cellThings.Count; i++)
                {
                    if (cellThings[i] is Pawn && cellThings[i] != instigator)
                    {
                        JPInjury.DoJPRelatedInjury(cellThings[i], true, true, instigator, fullDmg);
                        DFAEvent = true;
                    }
                }
            }
            return DFAEvent;
        }

        // Token: 0x06000047 RID: 71 RVA: 0x00003D24 File Offset: 0x00001F24
        public static void DoJPRelatedInjury(Thing victim, bool headSpace, bool isDFA, Thing instigator, bool fullDmg)
        {
            if (victim is Pawn)
            {
                JPInjury.SetUpInjVars(victim as Pawn, headSpace, isDFA, instigator, out BodyPartRecord candidate, out float injChance, out DamageDef DamDef, out float dmg);
                if (candidate != null && Rand.Range(1f, 100f) <= injChance)
                {
                    float armPen = 0f;
                    if (!fullDmg)
                    {
                        dmg = Math.Max(1f, dmg * (Settings.DFASplashFactor / 100f));
                    }
                    DamageInfo dinfo = new DamageInfo(DamDef, dmg, armPen, -1f, instigator, candidate, null, 0, null);
                    (victim as Pawn).TakeDamage(dinfo);
                }
            }
        }

        // Token: 0x06000048 RID: 72 RVA: 0x00003DAC File Offset: 0x00001FAC
        public static void SetUpInjVars(Pawn Victim, bool headspace, bool dfa, Thing instigator, out BodyPartRecord candidate, out float injuryChance, out DamageDef DamDef, out float dmg)
        {
            DamDef = null;
            int Rnd = Rand.Range(1, 100);
            if (Rnd < 50)
            {
                DamDef = DamageDefOf.Crush;
            }
            else if (Rnd >= 50 && Rnd < 75)
            {
                DamDef = DamageDefOf.Blunt;
            }
            else if (Rnd >= 75 && Rnd < 87)
            {
                DamDef = DamageDefOf.Stab;
            }
            else
            {
                DamDef = DamageDefOf.Stun;
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
            if (instigator != null && instigator is Pawn)
            {
                float bodyI = (instigator as Pawn).BodySize;
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
                float bodyV = Victim.BodySize;
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
            List<BodyPartRecord> potentials = new List<BodyPartRecord>();
            if (headspace)
            {
                BodyDef body = Victim.RaceProps.body;
                if ((body?.GetPartsWithDef(BodyPartDefOf.Head)) != null)
                {
                    potentials.AddRange(Victim.RaceProps.body.GetPartsWithDef(BodyPartDefOf.Head));
                }
                BodyDef body2 = Victim.RaceProps.body;
                if ((body2?.GetPartsWithDef(BodyPartDefOf.Neck)) != null)
                {
                    potentials.AddRange(Victim.RaceProps.body.GetPartsWithDef(BodyPartDefOf.Neck));
                }
                BodyDef body3 = Victim.RaceProps.body;
                if ((body3?.GetPartsWithDef(BodyPartDefOf.Arm)) != null)
                {
                    potentials.AddRange(Victim.RaceProps.body.GetPartsWithDef(BodyPartDefOf.Arm));
                }
                BodyDef body4 = Victim.RaceProps.body;
                if ((body4?.GetPartsWithDef(BodyPartDefOf.Torso)) != null)
                {
                    potentials.AddRange(Victim.RaceProps.body.GetPartsWithDef(BodyPartDefOf.Torso));
                }
                BodyDef body5 = Victim.RaceProps.body;
                if ((body5?.GetPartsWithDef(BodyPartDefOf.InsectHead)) != null)
                {
                    potentials.AddRange(Victim.RaceProps.body.GetPartsWithDef(BodyPartDefOf.InsectHead));
                }
            }
            else
            {
                BodyDef body6 = Victim.RaceProps.body;
                if ((body6?.GetPartsWithDef(BodyPartDefOf.Leg)) != null)
                {
                    potentials.AddRange(Victim.RaceProps.body.GetPartsWithDef(BodyPartDefOf.Leg));
                }
            }
            if (potentials.Count <= 0)
            {
                BodyDef body7 = Victim.RaceProps.body;
                if ((body7?.GetPartsWithDef(BodyPartDefOf.Body)) != null)
                {
                    potentials.AddRange(Victim.RaceProps.body.GetPartsWithDef(BodyPartDefOf.Body));
                }
            }
            if (potentials.Count > 0)
            {
                candidate = GenCollection.RandomElement<BodyPartRecord>(potentials);
            }
            injuryChance = 50f;
            if (dfa && headspace)
            {
                injuryChance = 75f;
            }
            if (instigator != null && instigator is Pawn)
            {
                Pawn pawn = instigator as Pawn;
                if ((pawn?.skills.GetSkill(SkillDefOf.Melee)) != null)
                {
                    int MeleeSkill = (instigator as Pawn).skills.GetSkill(SkillDefOf.Melee).levelInt;
                    injuryChance += ((float)MeleeSkill - 8f) / 2f;
                    dmg = Math.Max(dmg, dmg + ((float)MeleeSkill - 8f) / 3f);
                }
                Pawn pawn2 = instigator as Pawn;
                if ((pawn2?.skills.GetSkill(SkillDefOf.Shooting)) != null)
                {
                    int ShootingSkill = (instigator as Pawn).skills.GetSkill(SkillDefOf.Shooting).levelInt;
                    injuryChance += ((float)ShootingSkill - 10f) / 1.5f;
                    dmg = Math.Max(dmg, dmg + ((float)ShootingSkill - 10f) / 3f);
                }
            }
        }

        // Token: 0x04000022 RID: 34
        internal const float PEP = 0.33f;
    }
}
