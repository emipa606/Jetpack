using System;
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
            JPWeightUtility.GetTotalPilotWeight(pilot);
            JPWeightUtility.GetJPCarry(JP, fuel);
            float carryFactor = Mathf.Min(1f, JPWeightUtility.GetJPCarry(JP, fuel) / JPWeightUtility.GetTotalPilotWeight(pilot) * JPWeightUtility.GetGravityFactor());
            return Mathf.Lerp(0.5f, 1f, carryFactor);
        }

        // Token: 0x0600006A RID: 106 RVA: 0x0000528B File Offset: 0x0000348B
        public static float GetTotalPilotWeight(Pawn pilot)
        {
            return 0f + JPWeightUtility.GetPilotWeight(pilot) + JPWeightUtility.GetInventoryWeight(pilot, true) + JPWeightUtility.GetEquipmentWeight(pilot) + JPWeightUtility.GetApparelWeight(pilot);
        }

        // Token: 0x0600006B RID: 107 RVA: 0x000052B0 File Offset: 0x000034B0
        public static float GetPilotWeight(Pawn pilot)
        {
            float weight = 0f;
            float baseBodyWeight = 65f;
            if (pilot != null && pilot.BodySize > 0f)
            {
                weight += pilot.BodySize * baseBodyWeight;
            }
            return weight;
        }

        // Token: 0x0600006C RID: 108 RVA: 0x000052E8 File Offset: 0x000034E8
        public static float GetEquipmentWeight(Pawn pilot)
        {
            float weight = 0f;
            List<ThingWithComps> list;
            if (pilot == null)
            {
                list = null;
            }
            else
            {
                Pawn_EquipmentTracker equipment = pilot.equipment;
                list = (equipment?.AllEquipmentListForReading);
            }
            List<ThingWithComps> Eq = list;
            if (Eq != null && Eq.Count > 0)
            {
                for (int i = 0; i < Eq.Count; i++)
                {
                    weight += Eq[i].def.BaseMass * (float)Eq[i].stackCount;
                }
            }
            return weight;
        }

        // Token: 0x0600006D RID: 109 RVA: 0x00005354 File Offset: 0x00003554
        public static float GetInventoryWeight(Pawn pilot, bool includeCarried)
        {
            float weight = 0f;
            if (includeCarried)
            {
                Thing thing;
                if (pilot == null)
                {
                    thing = null;
                }
                else
                {
                    Pawn_CarryTracker carryTracker = pilot.carryTracker;
                    thing = (carryTracker?.CarriedThing);
                }
                Thing InvCarried = thing;
                if (InvCarried != null)
                {
                    weight += InvCarried.def.BaseMass * (float)InvCarried.stackCount;
                }
            }
            List<Thing> list;
            if (pilot == null)
            {
                list = null;
            }
            else
            {
                Pawn_InventoryTracker inventory = pilot.inventory;
                list = (inventory?.innerContainer.ToList<Thing>());
            }
            List<Thing> Inv = list;
            if (Inv != null && Inv.Count > 0)
            {
                for (int i = 0; i < Inv.Count; i++)
                {
                    weight += Inv[i].def.BaseMass * (float)Inv[i].stackCount;
                }
            }
            return weight;
        }

        // Token: 0x0600006E RID: 110 RVA: 0x000053FC File Offset: 0x000035FC
        public static float GetApparelWeight(Pawn pilot)
        {
            float weight = 0f;
            List<Apparel> list;
            if (pilot == null)
            {
                list = null;
            }
            else
            {
                Pawn_ApparelTracker apparel = pilot.apparel;
                list = (apparel?.WornApparel);
            }
            List<Apparel> App = list;
            if (App != null && App.Count > 0)
            {
                for (int i = 0; i < App.Count; i++)
                {
                    weight += App[i].def.BaseMass * (float)App[i].stackCount;
                }
            }
            return weight;
        }

        // Token: 0x0600006F RID: 111 RVA: 0x00005468 File Offset: 0x00003668
        public static float GetJPCarry(ThingDef JP, ThingDef fuel)
        {
            return JPWeightUtility.GetJPBaseCarry(JP) * JPWeightUtility.GetJPFuelCarryFactor(fuel);
        }

        // Token: 0x06000070 RID: 112 RVA: 0x00005478 File Offset: 0x00003678
        public static float GetJPBaseCarry(ThingDef JP)
        {
            float carry = 75f;
            string defName = JP.defName;
            if (!(defName == "Apparel_PowArmJetPack"))
            {
                if (!(defName == "Apparel_PowArmCGearJetPack"))
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
            float carryFactor = 1f;
            string defName = fuel.defName;
            if (!(defName == "MSHydrogenPeroxide"))
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
