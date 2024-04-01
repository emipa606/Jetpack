using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace JetPack;

public class JPInjury
{
    internal const float PEP = 0.33f;
    public static readonly BodyPartDef neck = DefDatabase<BodyPartDef>.GetNamedSilentFail("Neck");
    public static readonly BodyPartDef insectHead = DefDatabase<BodyPartDef>.GetNamedSilentFail("InsectHead");
    public static readonly BodyPartDef body = DefDatabase<BodyPartDef>.GetNamedSilentFail("Body");


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
        return Rand.Range(1, 100) < chance;
    }

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
        var dmg = (int)(fuel * GetBoomFactor(fueldef));
        GenExplosion.DoExplosion(pilot.Position, pilot.Map, radius, dmgdef, pilot, dmg, -1f, null, null, null,
            null, null, 1f, 2, null, false, preThingDef, 1f, 1, 0.9f, true);
        FleckMaker.ThrowSmoke(pilot.Position.ToVector3(), pilot.Map, 1f);
    }

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

    public static void DoJPRelatedInjury(Thing victim, bool headSpace, bool isDFA, Thing instigator, bool fullDmg)
    {
        if (victim is not Pawn pawn)
        {
            return;
        }

        SetUpInjVars(pawn, headSpace, isDFA, instigator, out var candidate, out var injChance,
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
        pawn.TakeDamage(dinfo);
    }

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
            var bodyDef = Victim?.RaceProps.body;
            if (bodyDef?.GetPartsWithDef(BodyPartDefOf.Head) != null)
            {
                potentials.AddRange(Victim.RaceProps.body.GetPartsWithDef(BodyPartDefOf.Head));
            }

            var body2 = Victim?.RaceProps.body;
            if (body2?.GetPartsWithDef(neck) != null)
            {
                potentials.AddRange(Victim.RaceProps.body.GetPartsWithDef(neck));
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
            if (body5?.GetPartsWithDef(insectHead) != null)
            {
                potentials.AddRange(Victim.RaceProps.body.GetPartsWithDef(insectHead));
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
            if (body7?.GetPartsWithDef(body) != null)
            {
                potentials.AddRange(Victim.RaceProps.body.GetPartsWithDef(body));
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

        if (instigator is not Pawn pawn)
        {
            return;
        }

        if (pawn.skills.GetSkill(SkillDefOf.Melee) != null)
        {
            var MeleeSkill = pawn.skills.GetSkill(SkillDefOf.Melee).levelInt;
            injuryChance += (MeleeSkill - 8f) / 2f;
            dmg = Math.Max(dmg, dmg + ((MeleeSkill - 8f) / 3f));
        }

        if (pawn.skills.GetSkill(SkillDefOf.Shooting) == null)
        {
            return;
        }

        var ShootingSkill = pawn.skills.GetSkill(SkillDefOf.Shooting).levelInt;
        injuryChance += (ShootingSkill - 10f) / 1.5f;
        dmg = Math.Max(dmg, dmg + ((ShootingSkill - 10f) / 3f));
    }
}