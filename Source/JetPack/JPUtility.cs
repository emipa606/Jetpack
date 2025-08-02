using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace JetPack;

public class JPUtility
{
    private const string JP_TCG = "Apparel_PowArmCGearJetPack";

    internal const string JP_POW = "Apparel_PowArmJetPack";

    internal const string JP_PAK = "Apparel_SpacerJetPack";

    private const string JP_BST = "Apparel_BoosterJetPack";

    internal const string JP_SW_JT12 = "JT-12_Jetpack";

    internal const string JP_SW_PazViz = "PazVizla_Jetpack";

    private const string SK_PAK = "SFJetPack";

    private const string SK_POW = "SFJetPackPowArm";

    private const string SK_TCG = "SFJetPackCGear";

    private const string SK_BST = "SFBoostPack";

    private const string JPFuel_Chemfuel = "Chemfuel";

    private const string JPFuel_Kerosene = "JPKerosene";

    private const string JPFuel_H2O2 = "MSHydrogenPeroxide";

    internal static Apparel GetWornJp(Thing pilot)
    {
        Apparel JP = null;
        bool b;
        if (pilot is not Pawn pawn)
        {
            b = false;
        }
        else
        {
            var apparel = pawn.apparel;
            var num = apparel != null ? new int?(apparel.WornApparelCount) : null;
            var num2 = 0;
            b = (num.GetValueOrDefault() > num2) & (num != null);
        }

        if (!b)
        {
            return null;
        }

        var list = new List<Apparel>();
        if (pilot is Pawn pawn2)
        {
            var apparel2 = pawn2.apparel;
            list = apparel2?.WornApparel;
        }

        var apparels = list;
        if (apparels is not { Count: > 0 })
        {
            return null;
        }

        foreach (var apparel in apparels)
        {
            if (apparel is not JetPackApparel)
            {
                continue;
            }

            JP = apparel;
            break;
        }

        return JP;
    }

    internal static bool IsInMeleeWithJp(Pawn pawn)
    {
        return pawn?.CurJob != null &&
               (pawn.CurJob.def == JobDefOf.AttackMelee || pawn.CurJob.def == JobDefOf.AttackStatic);
    }

    internal static bool GetIsJpApparel(ThingDef def)
    {
        string a;
        if (def == null)
        {
            a = null;
        }
        else
        {
            var thingClass = def.thingClass;
            a = thingClass?.FullName;
        }

        return a == "JetPack.JetPackApparel";
    }

    private static List<string> getSkyFallList()
    {
        var list = new List<string>();
        list.AddDistinct(SK_PAK);
        list.AddDistinct(SK_POW);
        list.AddDistinct(SK_TCG);
        list.AddDistinct(SK_BST);
        return list;
    }

    internal static List<string> ListFuelTypes(ThingDef JPDef)
    {
        var types = new List<string>
        {
            JPFuel_Chemfuel,
            JPFuel_Kerosene
        };
        if (JPDef.defName == JP_TCG &&
            DefDatabase<ThingDef>.GetNamed(JPFuel_H2O2, false) != null)
        {
            types.Add(JPFuel_H2O2);
        }

        return types;
    }

    public static int GetJumpRange(Pawn pawn, ThingDef JP, ThingDef fuel, float minRange)
    {
        var maxRange = getJumpRangeMax(fuel, JP);
        var carryFactor = 1f;
        if (Settings.UseCarry && pawn != null)
        {
            carryFactor = JPWeightUtility.JPCarryFactor(pawn, JP, fuel);
        }

        if (minRange > maxRange)
        {
            minRange = maxRange;
        }

        return (int)Mathf.Lerp(minRange, maxRange, carryFactor);
    }

    private static int getJumpRangeMax(ThingDef fuel, ThingDef JP)
    {
        var range = 20;
        var defName = fuel.defName;
        if (defName != JPFuel_H2O2)
        {
            if (defName == JPFuel_Kerosene)
            {
                range = 25;
            }
        }
        else
        {
            range = 23;
        }

        if (JP.defName == JP_BST)
        {
            range = 10;
        }

        return range;
    }

    internal static float GetSlowBurn(Pawn pilot)
    {
        var slowBurnMoveOffset = 0f;
        var JP = GetWornJp(pilot);
        if (JP == null)
        {
            return slowBurnMoveOffset;
        }

        if (((JetPackApparel)JP).JPSlowBurn)
        {
            slowBurnMoveOffset = 1.5f;
            var defName = (JP as JetPackApparel).JPFuelItem.defName;
            if (defName != JPFuel_H2O2)
            {
                if (defName == JPFuel_Kerosene)
                {
                    slowBurnMoveOffset *= 1.2f;
                }
            }
            else
            {
                slowBurnMoveOffset *= 1.1f;
            }
        }

        if (JP.def.defName == JP_BST)
        {
            slowBurnMoveOffset *= 2f;
        }

        return slowBurnMoveOffset;
    }

    internal static int GetSlowBurnTicks(ThingDef fuel)
    {
        return (int)(2500f / GetFuelRate(fuel));
    }

    internal static float GetFuelRate(ThingDef fuel)
    {
        var rate = 1f;
        var defName = fuel.defName;
        if (defName != JPFuel_H2O2)
        {
            if (defName == JPFuel_Kerosene)
            {
                rate = 0.5f;
            }
        }
        else
        {
            rate = 0.75f;
        }

        return rate;
    }

    internal static float GetIgnitionFactor(ThingDef fuel)
    {
        var factor = 10f;
        var defName = fuel.defName;
        if (defName != JPFuel_H2O2)
        {
            if (defName == JPFuel_Kerosene)
            {
                factor *= 1.2f;
            }
        }
        else
        {
            factor *= 0.8f;
        }

        return factor;
    }

    internal static bool ChkForDisallowed(Pawn pilot, out string reason)
    {
        reason = "";
        if (pilot.equipment.HasAnything())
        {
            var equip = pilot.equipment.AllEquipmentListForReading.ToList();
            if (equip.Count > 0)
            {
                foreach (var thingWithComps in equip)
                {
                    if (checkforHvy(thingWithComps) && !Settings.AllowHVY)
                    {
                        reason = "JetPack.DAllowEQHVY".Translate(thingWithComps.Label.CapitalizeFirst());
                        return true;
                    }

                    if (!chkforWmd(thingWithComps) || Settings.AllowWMD)
                    {
                        continue;
                    }

                    reason = "JetPack.DAllowEQWMD".Translate(thingWithComps.Label.CapitalizeFirst());
                    return true;
                }
            }
        }

        if (pilot.inventory.innerContainer.Any)
        {
            var things = pilot.inventory.innerContainer.InnerListForReading;
            if (things.Count > 0)
            {
                foreach (var thing in things)
                {
                    if (thing is not ThingWithComps comps)
                    {
                        continue;
                    }

                    if (checkforHvy(comps) && !Settings.AllowHVY)
                    {
                        reason = "JetPack.DAllowIVHVY".Translate(comps.Label.CapitalizeFirst());
                        return true;
                    }

                    if (!chkforWmd(comps) || Settings.AllowWMD)
                    {
                        continue;
                    }

                    reason = "JetPack.DAllowIVWMD".Translate(comps.Label.CapitalizeFirst());
                    return true;
                }
            }
        }

        var carryTracker = pilot.carryTracker;

        if (carryTracker?.CarriedThing == null)
        {
            return false;
        }

        var carry = pilot.carryTracker.CarriedThing;
        if (checkforHvy(carry as ThingWithComps) && !Settings.AllowHVY)
        {
            reason = "JetPack.DAllowCYHVY".Translate(carry.Label.CapitalizeFirst());
            return true;
        }

        if (!chkforWmd(carry as ThingWithComps) || Settings.AllowWMD)
        {
            return false;
        }

        reason = "JetPack.DAllowCYWMD".Translate(carry.Label.CapitalizeFirst());
        return true;
    }

    private static bool checkforHvy(ThingWithComps thing)
    {
        var def = thing.def;
        bool b;
        if (def == null)
        {
            b = false;
        }
        else
        {
            var weaponTags = def.weaponTags;
            bool? any;
            if (weaponTags == null)
            {
                any = null;
            }
            else
            {
                any = weaponTags.Any(x => x == "GunHeavy");
            }

            b = any.GetValueOrDefault() & (any != null);
        }

        if (b)
        {
            return true;
        }

        var def2 = thing.def;
        if (def2 is not { BaseMass: >= 5f })
        {
            return false;
        }

        var def3 = thing.def;
        if (def3?.Verbs == null)
        {
            return false;
        }

        foreach (var verbProperties in thing.def.Verbs)
        {
            if (verbProperties?.verbClass.Name == "Verb_ShootOneUse")
            {
                return true;
            }
        }

        return false;
    }

    private static bool chkforWmd(ThingWithComps thing)
    {
        var def = thing.def;
        if (def?.Verbs == null)
        {
            return false;
        }

        foreach (var verbProperties in thing.def.Verbs)
        {
            if (verbProperties is { ai_AvoidFriendlyFireRadius: >= 10 })
            {
                return true;
            }
        }

        return false;
    }

    internal static bool GetJPSkyFOnMap(Map map)
    {
        if (map == null)
        {
            return false;
        }

        var CheckList = map.listerThings.ThingsInGroup(ThingRequestGroup.ThingHolder);
        if (CheckList.Count <= 0)
        {
            return false;
        }

        var JPSkyFNames = getSkyFallList();
        foreach (var CheckThing in CheckList)
        {
            if (JPSkyFNames.Contains(CheckThing.def.defName))
            {
                return true;
            }
        }

        return false;
    }
}