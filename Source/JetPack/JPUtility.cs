using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace JetPack
{
    // Token: 0x02000013 RID: 19
    public class JPUtility
    {
        // Token: 0x04000028 RID: 40
        internal const string JP_TCG = "Apparel_PowArmCGearJetPack";

        // Token: 0x04000029 RID: 41
        internal const string JP_POW = "Apparel_PowArmJetPack";

        // Token: 0x0400002A RID: 42
        internal const string JP_PAK = "Apparel_SpacerJetPack";

        // Token: 0x0400002B RID: 43
        internal const string JP_BST = "Apparel_BoosterJetPack";

        // Token: 0x0400002C RID: 44
        internal const string JP_SW_JT12 = "JT-12_Jetpack";

        // Token: 0x0400002D RID: 45
        internal const string JP_SW_PazViz = "PazVizla_Jetpack";

        // Token: 0x0400002E RID: 46
        internal const string SK_PAK = "SFJetPack";

        // Token: 0x0400002F RID: 47
        internal const string SK_POW = "SFJetPackPowArm";

        // Token: 0x04000030 RID: 48
        internal const string SK_TCG = "SFJetPackCGear";

        // Token: 0x04000031 RID: 49
        internal const string SK_BST = "SFBoostPack";

        // Token: 0x04000032 RID: 50
        internal const string JPFuel_Chemfuel = "Chemfuel";

        // Token: 0x04000033 RID: 51
        internal const string JPFuel_Kerosene = "JPKerosene";

        // Token: 0x04000034 RID: 52
        internal const string JPFuel_H2O2 = "MSHydrogenPeroxide";

        // Token: 0x06000057 RID: 87 RVA: 0x000049B4 File Offset: 0x00002BB4
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

        // Token: 0x06000058 RID: 88 RVA: 0x00004A66 File Offset: 0x00002C66
        internal static bool IsInMeleeWithJP(Pawn pawn)
        {
            return pawn?.CurJob != null &&
                   (pawn.CurJob.def == JobDefOf.AttackMelee || pawn.CurJob.def == JobDefOf.AttackStatic);
        }

        // Token: 0x06000059 RID: 89 RVA: 0x00004A9D File Offset: 0x00002C9D
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

        // Token: 0x0600005A RID: 90 RVA: 0x00004AC6 File Offset: 0x00002CC6
        internal static List<string> GetSkyFallList()
        {
            var list = new List<string>();
            list.AddDistinct("SFJetPack");
            list.AddDistinct("SFJetPackPowArm");
            list.AddDistinct("SFJetPackCGear");
            list.AddDistinct("SFBoostPack");
            return list;
        }

        // Token: 0x0600005B RID: 91 RVA: 0x00004AFC File Offset: 0x00002CFC
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

        // Token: 0x0600005C RID: 92 RVA: 0x00004B50 File Offset: 0x00002D50
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

            return (int) Mathf.Lerp(minRange, maxRange, carryFactor);
        }

        // Token: 0x0600005D RID: 93 RVA: 0x00004B94 File Offset: 0x00002D94
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

        // Token: 0x0600005E RID: 94 RVA: 0x00004BE8 File Offset: 0x00002DE8
        internal static float GetSlowBurn(Pawn pilot)
        {
            var slowBurnMoveOffset = 0f;
            var JP = GetWornJP(pilot);
            if (JP == null)
            {
                return slowBurnMoveOffset;
            }

            if (((JetPackApparel) JP).JPSlowBurn)
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

        // Token: 0x0600005F RID: 95 RVA: 0x00004C77 File Offset: 0x00002E77
        internal static int GetSlowBurnTicks(ThingDef fuel)
        {
            return (int) (2500f / GetFuelRate(fuel));
        }

        // Token: 0x06000060 RID: 96 RVA: 0x00004C88 File Offset: 0x00002E88
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

        // Token: 0x06000061 RID: 97 RVA: 0x00004CD0 File Offset: 0x00002ED0
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

        // Token: 0x06000062 RID: 98 RVA: 0x00004D1C File Offset: 0x00002F1C
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

        // Token: 0x06000063 RID: 99 RVA: 0x00004D68 File Offset: 0x00002F68
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
                        if (thing is not ThingWithComps)
                        {
                            continue;
                        }

                        if (ChkforHVY(thing as ThingWithComps) && !Settings.AllowHVY)
                        {
                            reason = "JetPack.DAllowIVHVY".Translate(thing.Label.CapitalizeFirst());
                            return true;
                        }

                        if (!ChkforWMD(thing as ThingWithComps) || Settings.AllowWMD)
                        {
                            continue;
                        }

                        reason = "JetPack.DAllowIVWMD".Translate(thing.Label.CapitalizeFirst());
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

        // Token: 0x06000064 RID: 100 RVA: 0x00004FB8 File Offset: 0x000031B8
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

        // Token: 0x06000065 RID: 101 RVA: 0x000050C4 File Offset: 0x000032C4
        internal static bool ChkforWMD(ThingWithComps thing)
        {
            var def = thing.def;
            if (def?.Verbs == null)
            {
                return false;
            }

            foreach (var verbProperties in thing.def.Verbs)
            {
                if (verbProperties != null && verbProperties.ai_AvoidFriendlyFireRadius >= 10)
                {
                    return true;
                }
            }

            return false;
        }

        // Token: 0x06000066 RID: 102 RVA: 0x00005148 File Offset: 0x00003348
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

        // Token: 0x06000067 RID: 103 RVA: 0x000051AC File Offset: 0x000033AC
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
}