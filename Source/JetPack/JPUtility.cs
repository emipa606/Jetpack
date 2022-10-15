using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace JetPack;

public class JPUtility
{
    internal const string JP_TCG = "Apparel_PowArmCGearJetPack";

    internal const string JP_POW = "Apparel_PowArmJetPack";

    internal const string JP_PAK = "Apparel_SpacerJetPack";

    internal const string JP_BST = "Apparel_BoosterJetPack";

    internal const string JP_SW_JT12 = "JT-12_Jetpack";

    internal const string JP_SW_PazViz = "PazVizla_Jetpack";

    internal const string SK_PAK = "SFJetPack";

    internal const string SK_POW = "SFJetPackPowArm";

    internal const string SK_TCG = "SFJetPackCGear";

    internal const string SK_BST = "SFBoostPack";

    internal const string JPFuel_Chemfuel = "Chemfuel";

    internal const string JPFuel_Kerosene = "JPKerosene";

    internal const string JPFuel_H2O2 = "MSHydrogenPeroxide";

    internal static Apparel GetWornJP(Thing pilot)
    {
        Apparel JP = null;
        bool b;
        if (!(pilot is Pawn pawn))
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
        if (apparels == null || apparels.Count <= 0)
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

    internal static bool IsInMeleeWithJP(Pawn pawn)
    {
        return pawn?.CurJob != null &&
               (pawn.CurJob.def == JobDefOf.AttackMelee || pawn.CurJob.def == JobDefOf.AttackStatic);
    }

    internal static bool GetIsJPApparel(ThingDef def)
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

    internal static List<string> GetSkyFallList()
    {
        var list = new List<string>();
        list.AddDistinct("SFJetPack");
        list.AddDistinct("SFJetPackPowArm");
        list.AddDistinct("SFJetPackCGear");
        list.AddDistinct("SFBoostPack");
        return list;
    }

    internal static List<string> ListFuelTypes(ThingDef JPDef)
    {
        var types = new List<string>
        {
            "Chemfuel",
            "JPKerosene"
        };
        if (JPDef.defName == "Apparel_PowArmCGearJetPack" &&
            DefDatabase<ThingDef>.GetNamed("MSHydrogenPeroxide", false) != null)
        {
            types.Add("MSHydrogenPeroxide");
        }

        return types;
    }

    public static int GetJumpRange(Pawn pawn, ThingDef JP, ThingDef fuel, float minRange)
    {
        var maxRange = GetJumpRangeMax(fuel, JP);
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

    internal static int GetJumpRangeMax(ThingDef fuel, ThingDef JP)
    {
        var range = 20;
        var defName = fuel.defName;
        if (defName != "MSHydrogenPeroxide")
        {
            if (defName == "JPKerosene")
            {
                range = 25;
            }
        }
        else
        {
            range = 23;
        }

        if (JP.defName == "Apparel_BoosterJetPack")
        {
            range = 10;
        }

        return range;
    }

    internal static float GetSlowBurn(Pawn pilot)
    {
        var slowBurnMoveOffset = 0f;
        var JP = GetWornJP(pilot);
        if (JP == null)
        {
            return slowBurnMoveOffset;
        }

        if (((JetPackApparel)JP).JPSlowBurn)
        {
            slowBurnMoveOffset = 1.5f;
            var defName = (JP as JetPackApparel).JPFuelItem.defName;
            if (defName != "MSHydrogenPeroxide")
            {
                if (defName == "JPKerosene")
                {
                    slowBurnMoveOffset *= 1.2f;
                }
            }
            else
            {
                slowBurnMoveOffset *= 1.1f;
            }
        }

        if (JP.def.defName == "Apparel_BoosterJetPack")
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
        if (defName != "MSHydrogenPeroxide")
        {
            if (defName == "JPKerosene")
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

    internal static float GetMassCapacity(ThingDef fuel)
    {
        var MC = 70f;
        var defName = fuel.defName;
        if (defName != "MSHydrogenPeroxide")
        {
            if (defName == "JPKerosene")
            {
                MC *= 2f;
            }
        }
        else
        {
            MC *= 1.5f;
        }

        return MC;
    }

    internal static float GetIgnitionFactor(ThingDef fuel)
    {
        var factor = 10f;
        var defName = fuel.defName;
        if (defName != "MSHydrogenPeroxide")
        {
            if (defName == "JPKerosene")
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

    internal static bool ChkForDissallowed(Pawn pilot, out string reason)
    {
        reason = "";
        if (pilot.equipment.HasAnything())
        {
            var equip = pilot.equipment.AllEquipmentListForReading.ToList();
            if (equip.Count > 0)
            {
                foreach (var thingWithComps in equip)
                {
                    if (ChkforHVY(thingWithComps) && !Settings.AllowHVY)
                    {
                        reason = "JetPack.DAllowEQHVY".Translate(thingWithComps.Label.CapitalizeFirst());
                        return true;
                    }

                    if (!ChkforWMD(thingWithComps) || Settings.AllowWMD)
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

                    if (ChkforHVY(comps) && !Settings.AllowHVY)
                    {
                        reason = "JetPack.DAllowIVHVY".Translate(comps.Label.CapitalizeFirst());
                        return true;
                    }

                    if (!ChkforWMD(comps) || Settings.AllowWMD)
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
        if (ChkforHVY(carry as ThingWithComps) && !Settings.AllowHVY)
        {
            reason = "JetPack.DAllowCYHVY".Translate(carry.Label.CapitalizeFirst());
            return true;
        }

        if (!ChkforWMD(carry as ThingWithComps) || Settings.AllowWMD)
        {
            return false;
        }

        reason = "JetPack.DAllowCYWMD".Translate(carry.Label.CapitalizeFirst());
        return true;
    }

    internal static bool ChkforHVY(ThingWithComps thing)
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
        if (def2 == null || !(def2.BaseMass >= 5f))
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

    internal static bool ChkforWMD(ThingWithComps thing)
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

    internal static Pawn GetSkyFPilot(Thing t)
    {
        Thing pilot = null;
        var skf = t as Skyfaller;
        if (skf != null && (!skf.innerContainer.Any || skf.innerContainer.Count <= 0))
        {
            return null;
        }

        if (skf?.innerContainer == null)
        {
            return null;
        }

        foreach (var thingchk in skf.innerContainer)
        {
            if (thingchk is Pawn)
            {
                pilot = thingchk;
            }
        }

        return pilot as Pawn;
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

        var JPSkyFNames = GetSkyFallList();
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