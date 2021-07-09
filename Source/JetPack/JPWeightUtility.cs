using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace JetPack
{
    // Token: 0x02000014 RID: 20
    public class JPWeightUtility
    {
        // Token: 0x06000069 RID: 105 RVA: 0x00005238 File Offset: 0x00003438
        public static float JPCarryFactor(Pawn pilot, ThingDef JP, ThingDef fuel)
        {
            GetTotalPilotWeight(pilot);
            GetJPCarry(JP, fuel);
            var carryFactor = Mathf.Min(1f, GetJPCarry(JP, fuel) / GetTotalPilotWeight(pilot) * GetGravityFactor());
            return Mathf.Lerp(0.5f, 1f, carryFactor);
        }

        // Token: 0x0600006A RID: 106 RVA: 0x0000528B File Offset: 0x0000348B
        public static float GetTotalPilotWeight(Pawn pilot)
        {
            return 0f + GetPilotWeight(pilot) + GetInventoryWeight(pilot, true) + GetEquipmentWeight(pilot) +
                   GetApparelWeight(pilot);
        }

        // Token: 0x0600006B RID: 107 RVA: 0x000052B0 File Offset: 0x000034B0
        public static float GetPilotWeight(Pawn pilot)
        {
            var weight = 0f;
            var baseBodyWeight = 65f;
            if (pilot != null && pilot.BodySize > 0f)
            {
                weight += pilot.BodySize * baseBodyWeight;
            }

            return weight;
        }

        // Token: 0x0600006C RID: 108 RVA: 0x000052E8 File Offset: 0x000034E8
        public static float GetEquipmentWeight(Pawn pilot)
        {
            var weight = 0f;
            List<ThingWithComps> list;
            if (pilot == null)
            {
                list = null;
            }
            else
            {
                var equipment = pilot.equipment;
                list = equipment?.AllEquipmentListForReading;
            }

            var Eq = list;
            if (Eq == null || Eq.Count <= 0)
            {
                return weight;
            }

            foreach (var thingWithComps in Eq)
            {
                weight += thingWithComps.def.BaseMass * thingWithComps.stackCount;
            }

            return weight;
        }

        // Token: 0x0600006D RID: 109 RVA: 0x00005354 File Offset: 0x00003554
        public static float GetInventoryWeight(Pawn pilot, bool includeCarried)
        {
            var weight = 0f;
            if (includeCarried)
            {
                Thing thing;
                if (pilot == null)
                {
                    thing = null;
                }
                else
                {
                    var carryTracker = pilot.carryTracker;
                    thing = carryTracker?.CarriedThing;
                }

                var InvCarried = thing;
                if (InvCarried != null)
                {
                    weight += InvCarried.def.BaseMass * InvCarried.stackCount;
                }
            }

            List<Thing> list;
            if (pilot == null)
            {
                list = null;
            }
            else
            {
                var inventory = pilot.inventory;
                list = inventory?.innerContainer.ToList();
            }

            var Inv = list;
            if (Inv == null || Inv.Count <= 0)
            {
                return weight;
            }

            foreach (var thing in Inv)
            {
                weight += thing.def.BaseMass * thing.stackCount;
            }

            return weight;
        }

        // Token: 0x0600006E RID: 110 RVA: 0x000053FC File Offset: 0x000035FC
        public static float GetApparelWeight(Pawn pilot)
        {
            var weight = 0f;
            List<Apparel> list;
            if (pilot == null)
            {
                list = null;
            }
            else
            {
                var apparel = pilot.apparel;
                list = apparel?.WornApparel;
            }

            var App = list;
            if (App == null || App.Count <= 0)
            {
                return weight;
            }

            foreach (var apparel in App)
            {
                weight += apparel.def.BaseMass * apparel.stackCount;
            }

            return weight;
        }

        // Token: 0x0600006F RID: 111 RVA: 0x00005468 File Offset: 0x00003668
        public static float GetJPCarry(ThingDef JP, ThingDef fuel)
        {
            return GetJPBaseCarry(JP) * GetJPFuelCarryFactor(fuel);
        }

        // Token: 0x06000070 RID: 112 RVA: 0x00005478 File Offset: 0x00003678
        public static float GetJPBaseCarry(ThingDef JP)
        {
            var carry = 75f;
            var defName = JP.defName;
            if (defName != "Apparel_PowArmJetPack")
            {
                if (defName != "Apparel_PowArmCGearJetPack")
                {
                    if (defName == "Apparel_BoosterJetPack")
                    {
                        carry += -20f;
                    }
                }
                else
                {
                    carry += 7f;
                }
            }
            else
            {
                carry += 5f;
            }

            return carry;
        }

        // Token: 0x06000071 RID: 113 RVA: 0x000054D8 File Offset: 0x000036D8
        public static float GetJPFuelCarryFactor(ThingDef fuel)
        {
            var carryFactor = 1f;
            var defName = fuel.defName;
            if (defName != "MSHydrogenPeroxide")
            {
                if (defName == "JPKerosene")
                {
                    carryFactor = 1.25f;
                }
            }
            else
            {
                carryFactor = 1.1f;
            }

            return carryFactor;
        }

        // Token: 0x06000072 RID: 114 RVA: 0x0000551D File Offset: 0x0000371D
        public static float GetGravityFactor()
        {
            return 1f;
        }
    }
}