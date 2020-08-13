using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace JetPack
{
    // Token: 0x0200000D RID: 13
    public class JetPackApparel : Apparel
    {
        // Token: 0x17000003 RID: 3
        // (get) Token: 0x0600001F RID: 31 RVA: 0x000025C8 File Offset: 0x000007C8
        public CompJetPack JetPackComp
        {
            get
            {
                return base.GetComp<CompJetPack>();
            }
        }

        // Token: 0x17000004 RID: 4
        // (get) Token: 0x06000020 RID: 32 RVA: 0x000025D0 File Offset: 0x000007D0
        public bool JPOnCooldown
        {
            get
            {
                return this.JPCooldownTicks > 0;
            }
        }

        // Token: 0x06000021 RID: 33 RVA: 0x000025DC File Offset: 0x000007DC
        public override void PostMake()
        {
            base.PostMake();
            this.JPFuelItem = DefDatabase<ThingDef>.GetNamed(base.GetComp<CompJetPack>().Props.JPFuel, false);
            this.JPFuelAmount = base.GetComp<CompJetPack>().Props.JPFuelLevel;
            this.JPFuelMax = base.GetComp<CompJetPack>().Props.JPFuelMaximum;
            this.JPFuelMin = base.GetComp<CompJetPack>().Props.JPFuelMinimum;
            this.JPFuelRate = base.GetComp<CompJetPack>().Props.JPFuelBurnRate;
            this.JPJumpRangeMax = base.GetComp<CompJetPack>().Props.JPJumpMax;
            this.JPJumpRangeMin = base.GetComp<CompJetPack>().Props.JPJumpMin;
            this.JPSkyFallType = base.GetComp<CompJetPack>().Props.JPSKType;
            this.JPPilotIsDrafted = false;
            this.JPSlowBurn = false;
            this.JPSlowBurnTicks = 0;
            if (this.JPFuelItem == null)
            {
                this.JPFuelItem = ThingDefOf.Chemfuel;
                Log.Message("Warning: Jet pack fuel item not defined, defaulting to chemfuel", false);
            }
            else
            {
                Pawn pilot = base.Wearer;
                if (this.JPFuelItem == DefDatabase<ThingDef>.GetNamed("MSHydrogenPeroxide", false))
                {
                    this.JPJumpRangeMax = (float)JPUtility.GetJumpRange(pilot, this.def, this.JPFuelItem, this.JPJumpRangeMin);
                    this.JPFuelRate = JPUtility.GetFuelRate(this.JPFuelItem);
                }
                else if (this.JPFuelItem == DefDatabase<ThingDef>.GetNamed("JPKerosene", true))
                {
                    this.JPJumpRangeMax = (float)JPUtility.GetJumpRange(pilot, this.def, this.JPFuelItem, this.JPJumpRangeMin);
                    this.JPFuelRate = JPUtility.GetFuelRate(this.JPFuelItem);
                }
            }
            if (this.JPFuelAmount < 0)
            {
                this.JPFuelAmount = 0;
            }
            if (this.JPFuelMax < 75)
            {
                this.JPFuelMax = 75;
            }
            if (this.JPFuelMax > 150)
            {
                this.JPFuelMax = 150;
            }
            if (this.JPFuelMin < 5)
            {
                this.JPFuelMin = 5;
            }
            if (this.JPFuelMin > this.JPFuelMax)
            {
                this.JPFuelMin = this.JPFuelMax;
            }
            if (this.JPFuelRate < 0.1f)
            {
                this.JPFuelRate = 0.1f;
            }
            if (this.JPFuelRate > 1f)
            {
                this.JPFuelRate = 1f;
            }
            if (this.JPJumpRangeMax > 30f)
            {
                this.JPJumpRangeMax = 30f;
            }
            if (this.JPJumpRangeMax < 10f)
            {
                this.JPJumpRangeMax = 10f;
            }
            if (this.JPJumpRangeMin < 5f)
            {
                this.JPJumpRangeMin = 5f;
            }
            if (this.JPJumpRangeMin > 9f)
            {
                this.JPJumpRangeMin = 9f;
            }
            if (this.def.defName != "Apparel_PowArmCGearJetPack")
            {
                if (this.def.defName != "Apparel_PowArmJetPack")
                {
                    if (this.def.defName != "Apparel_BoosterJetPack")
                    {
                        if (this.JPSkyFallType != "SFJetPack")
                        {
                            this.JPSkyFallType = "SFJetPack";
                        }
                    }
                    else
                    {
                        this.JPSkyFallType = "SFBoostPack";
                    }
                }
                else
                {
                    this.JPSkyFallType = "SFJetPackPowArm";
                }
            }
            else
            {
                this.JPSkyFallType = "SFJetPackCGear";
            }
            if (this.JPFuelAmount > this.JPFuelMax)
            {
                this.JPFuelAmount = this.JPFuelMax;
            }
        }

        // Token: 0x06000022 RID: 34 RVA: 0x00002904 File Offset: 0x00000B04
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Defs.Look<ThingDef>(ref this.JPFuelItem, "JPFuelItem");
            Scribe_Values.Look<int>(ref this.JPFuelAmount, "JPFuelAmount", 0, false);
            Scribe_Values.Look<int>(ref this.JPFuelMax, "JPFuelMax", 150, false);
            Scribe_Values.Look<int>(ref this.JPFuelMin, "JPFuelMin", 10, false);
            Scribe_Values.Look<float>(ref this.JPFuelRate, "JPFuelRate", 1f, false);
            Scribe_Values.Look<float>(ref this.JPJumpRangeMax, "JPJumpRangeMax", 25f, false);
            Scribe_Values.Look<float>(ref this.JPJumpRangeMin, "JPJumpRangeMin", 10f, false);
            Scribe_Values.Look<bool>(ref this.JPPilotIsDrafted, "JPPilotIsDrafted", false, false);
            Scribe_Values.Look<string>(ref this.JPSkyFallType, "JPSkyFallType", "SFJetPack", false);
            Scribe_Values.Look<bool>(ref this.JPSlowBurn, "JPSlowBurn", false, false);
            Scribe_Values.Look<int>(ref this.JPSlowBurnTicks, "JPSlowBurnTicks", 0, false);
        }

        // Token: 0x06000023 RID: 35 RVA: 0x000029F0 File Offset: 0x00000BF0
        public override void Tick()
        {
            base.Tick();
            if (this.JPCooldownTicks > 0)
            {
                this.JPCooldownTicks--;
            }
            if (Settings.AllowSlowBurn)
            {
                if (base.Wearer == null)
                {
                    this.JPSlowBurn = false;
                    return;
                }
                if (this.JPSlowBurn && base.Wearer.Spawned && PawnUtility.GetPosture(base.Wearer) != PawnPosture.LayingInBed && PawnUtility.GetPosture(base.Wearer) != PawnPosture.LayingOnGroundFaceUp && PawnUtility.GetPosture(base.Wearer) != PawnPosture.LayingOnGroundNormal)
                {
                    if (this.JPSlowBurnTicks > 0)
                    {
                        this.JPSlowBurnTicks--;
                        return;
                    }
                    if (this.JPFuelAmount > 0)
                    {
                        this.JPFuelAmount--;
                        this.JPSlowBurnTicks = JPUtility.GetSlowBurnTicks(this.JPFuelItem);
                        return;
                    }
                    this.JPSlowBurn = false;
                    Messages.Message(TranslatorFormattedStringExtensions.Translate("JetPack.SBOutOfFuel", GenText.CapitalizeFirst(base.Wearer.LabelShort), GenText.CapitalizeFirst(this.Label)), base.Wearer, MessageTypeDefOf.NeutralEvent, false);
                    return;
                }
            }
            else
            {
                this.JPSlowBurn = false;
            }
        }

        // Token: 0x06000024 RID: 36 RVA: 0x00002B1F File Offset: 0x00000D1F
        public override void TickLong()
        {
            base.TickLong();
        }

        // Token: 0x06000025 RID: 37 RVA: 0x00002B27 File Offset: 0x00000D27
        public override IEnumerable<Gizmo> GetWornGizmos()
        {
            Texture2D fuelicon = this.JPFuelItem.uiIcon;
            if (fuelicon == null)
            {
                ThingDef chkFuelItem = null;
                if (this.JPFuelItem == null)
                {
                    chkFuelItem = DefDatabase<ThingDef>.GetNamed(base.GetComp<CompJetPack>().Props.JPFuel, false);
                }
                if (chkFuelItem != null && chkFuelItem != this.JPFuelItem)
                {
                    this.JPFuelItem = chkFuelItem;
                    int chkFuelMax = base.GetComp<CompJetPack>().Props.JPFuelMaximum;
                    if (chkFuelMax != this.JPFuelMax)
                    {
                        this.JPFuelMax = chkFuelMax;
                        if (this.JPFuelAmount > this.JPFuelMax)
                        {
                            this.JPFuelAmount = this.JPFuelMax;
                        }
                    }
                }
                fuelicon = this.JPFuelItem.uiIcon;
                if (fuelicon == null)
                {
                    this.JPFuelItem = ThingDefOf.Chemfuel;
                    fuelicon = this.JPFuelItem.uiIcon;
                    if (fuelicon == null)
                    {
                        fuelicon = this.def.uiIcon;
                        Log.Message("Error: Jet Pack fuel definition not found.", false);
                    }
                }
            }
            string chkSkyFallType = base.GetComp<CompJetPack>().Props.JPSKType;
            if (chkSkyFallType == null || chkSkyFallType == "")
            {
                this.JPSkyFallType = "SFJetPack";
            }
            else if (chkSkyFallType != this.JPSkyFallType)
            {
                this.JPSkyFallType = chkSkyFallType;
            }
            Pawn pilot = base.Wearer;
            this.JPJumpRangeMax = (float)JPUtility.GetJumpRange(pilot, this.def, this.JPFuelItem, this.JPJumpRangeMin);
            this.JPFuelRate = JPUtility.GetFuelRate(this.JPFuelItem);
            if (base.Wearer != null && base.Wearer.IsColonistPlayerControlled)
            {
                Pawn wearer = base.Wearer;
                if ((wearer?.Map) != null && !base.Wearer.Downed && base.Wearer.Spawned && !JPUtility.IsInMeleeWithJP(base.Wearer))
                {
                    if (Find.Selector.SingleSelectedThing == base.Wearer)
                    {
                        string text;
                        if (this.JPOnCooldown)
                        {
                            text = TranslatorFormattedStringExtensions.Translate("JetPack.CooldownTicks", this.JPCooldownTicks.ToString());
                        }
                        else
                        {
                            text = Translator.Translate("JetPack.JPJump");
                        }
                        string desc = TranslatorFormattedStringExtensions.Translate("JetPack.JPDesc", GenText.CapitalizeFirst(this.def.label));
                        yield return new Command_JetPack
                        {
                            defaultLabel = text,
                            defaultDesc = desc,
                            icon = this.def.uiIcon,
                            Pilot = base.Wearer,
                            JPFuel = this.JPFuelAmount,
                            JPFRate = this.JPFuelRate,
                            JPMaxJump = this.JPJumpRangeMax,
                            JPMinJump = this.JPJumpRangeMin,
                            JPSKFStr = this.JPSkyFallType,
                            action = delegate (IntVec3 cell)
                            {
                                SoundStarter.PlayOneShotOnCamera(SoundDefOf.Click, null);
                                this.UseJetPack(base.Wearer, this, cell);
                            }
                        };
                        if (Settings.AllowSlowBurn)
                        {
                            text = Translator.Translate("JetPack.JPSBSet");
                            desc = Translator.Translate("JetPack.JPSBDesc");
                            yield return new Command_Toggle
                            {
                                icon = ContentFinder<Texture2D>.Get(this.JPSBTexPath, true),
                                defaultLabel = text,
                                defaultDesc = desc,
                                isActive = (() => this.JPSlowBurn),
                                toggleAction = delegate ()
                                {
                                    this.ToggleSlowBurn(this.JPSlowBurn);
                                }
                            };
                        }
                        text = TranslatorFormattedStringExtensions.Translate("JetPack.JPLabel", GenText.CapitalizeFirst(this.def.label), this.JPFuelAmount.ToString(), this.JPFuelMax.ToString());
                        desc = TranslatorFormattedStringExtensions.Translate("JetPack.JPDesc", GenText.CapitalizeFirst(this.def.label));
                        yield return new Command_Action
                        {
                            defaultLabel = text,
                            defaultDesc = desc,
                            icon = fuelicon,
                            action = delegate ()
                            {
                                SoundStarter.PlayOneShotOnCamera(SoundDefOf.Click, null);
                                this.RefuelJetPack(base.Wearer, this);
                            }
                        };
                        if (Prefs.DevMode)
                        {
                            yield return new Command_Action
                            {
                                defaultLabel = "Debug Settings",
                                defaultDesc = "Debug Commands",
                                icon = fuelicon,
                                action = delegate ()
                                {
                                    SoundStarter.PlayOneShotOnCamera(SoundDefOf.Click, null);
                                    this.DoJetPackDebug(base.Wearer, this);
                                }
                            };
                        }
                    }
                    yield break;
                }
            }
            yield break;
        }

        // Token: 0x06000026 RID: 38 RVA: 0x00002B37 File Offset: 0x00000D37
        public void ToggleSlowBurn(bool flag)
        {
            this.JPSlowBurn = !flag;
        }

        // Token: 0x06000027 RID: 39 RVA: 0x00002B44 File Offset: 0x00000D44
        public void UseJetPack(Pawn pilot, Thing JP, IntVec3 targCell)
        {
            if (!this.JPComposMentis(pilot, JP, out string Reason))
            {
                Messages.Message(TranslatorFormattedStringExtensions.Translate("JetPack.CantDo", pilot, Reason), pilot, MessageTypeDefOf.NeutralEvent, false);
                SoundStarter.PlayOneShotOnCamera(SoundDefOf.ClickReject, null);
                return;
            }
            if (!this.FlightChecksOK(pilot, JP, out string ChecksReason))
            {
                Messages.Message(TranslatorFormattedStringExtensions.Translate("JetPack.ChecksReason", GenText.CapitalizeFirst(pilot.LabelShort), ChecksReason), pilot, MessageTypeDefOf.NeutralEvent, false);
                SoundStarter.PlayOneShotOnCamera(SoundDefOf.ClickReject, null);
                return;
            }
            if (JPUtility.ChkForDissallowed(pilot, out string DAllowReason))
            {
                Messages.Message(TranslatorFormattedStringExtensions.Translate("JetPack.DAllowReason", GenText.CapitalizeFirst(pilot.LabelShort), DAllowReason), pilot, MessageTypeDefOf.NeutralEvent, false);
                SoundStarter.PlayOneShotOnCamera(SoundDefOf.ClickReject, null);
                return;
            }
            if (!this.FlightCellCheck(pilot, targCell, this.JPFuelAmount, this.JPFuelRate, this.JPJumpRangeMin, this.JPJumpRangeMax, out string JumpReason))
            {
                Messages.Message(TranslatorFormattedStringExtensions.Translate("JetPack.JumpReason", JumpReason), pilot, MessageTypeDefOf.NeutralEvent, false);
                SoundStarter.PlayOneShotOnCamera(SoundDefOf.ClickReject, null);
                return;
            }
            if (!JPInjury.CheckForExplosion(this))
            {
                this.DoJumpJet(pilot, targCell);
                return;
            }
            JPInjury.DoJPExplosion(pilot, this.JPFuelAmount, this.JPFuelItem);
            this.JPFuelAmount = 0;
        }

        // Token: 0x06000028 RID: 40 RVA: 0x00002CB8 File Offset: 0x00000EB8
        public void DoJumpJet(Pawn Pilot, IntVec3 targCell)
        {
            this.JPRotatePilot(Pilot, targCell, out float angle);
            if (this.JPSkyFallType == null)
            {
                this.JPSkyFallType = "SFJetPack";
            }
            ThingDef JPSF = DefDatabase<ThingDef>.GetNamed(this.JPSkyFallType, false);
            if (JPSF == null)
            {
                JPSF = DefDatabase<ThingDef>.GetNamed("SFJetPack", true);
                this.JPSkyFallType = "SFJetPack";
            }
            float SFspeed = JPSF.skyfaller.speed;
            if (SFspeed <= 0f)
            {
                SFspeed = 1f;
            }
            float distance = IntVec3Utility.DistanceTo(Pilot.Position, targCell);
            int timeToLand = (int)(distance / SFspeed);
            angle += 270f;
            if (angle >= 360f)
            {
                angle -= 360f;
            }
            int fuelUsed;
            if (Settings.UseCarry)
            {
                fuelUsed = (int)Math.Floor((double)(distance * this.JPFuelRate / JPWeightUtility.JPCarryFactor(Pilot, this.def, this.JPFuelItem)));
            }
            else
            {
                fuelUsed = (int)Math.Floor((double)(distance * this.JPFuelRate));
            }
            this.JPFuelAmount -= fuelUsed;
            Skyfaller skyfaller = SkyfallerMaker.SpawnSkyfaller(JPSF, targCell, Pilot.Map);
            skyfaller.ticksToImpact = timeToLand;
            skyfaller.angle = angle;
            this.JPPilotIsDrafted = Pilot.Drafted;
            Pilot.DeSpawn(0);
            skyfaller.innerContainer.TryAdd(Pilot, false);
        }

        // Token: 0x06000029 RID: 41 RVA: 0x00002DD8 File Offset: 0x00000FD8
        internal void JPRotatePilot(Pawn pilot, IntVec3 targCell, out float angle)
        {
            angle = Vector3Utility.AngleToFlat(pilot.Position.ToVector3(), targCell.ToVector3());
            float offsetAngle = angle + 90f;
            if (offsetAngle > 360f)
            {
                offsetAngle -= 360f;
            }
            Rot4 facing = Rot4.FromAngleFlat(offsetAngle);
            pilot.Rotation = facing;
        }

        // Token: 0x0600002A RID: 42 RVA: 0x00002E28 File Offset: 0x00001028
        internal bool FlightCellCheck(Pawn pilot, IntVec3 tCell, int fuel, float fRate, float minJump, float maxJump, out string cantReason)
        {
            cantReason = "";
            if (!GenGrid.InBounds(tCell, pilot.Map))
            {
                cantReason = Translator.Translate("JetPack.JumpReasonInvalid");
                return false;
            }
            if (this.JPOnCooldown)
            {
                cantReason = Translator.Translate("JetPack.JumpReasonOnCooldown");
                return false;
            }
            if (!ReservationUtility.CanReserve(pilot, tCell, 1, -1, null, false))
            {
                cantReason = Translator.Translate("JetPack.JumpReasonReserved");
                return false;
            }
            if (GridsUtility.Roofed(tCell, pilot.Map))
            {
                if (!Settings.RoofPunch)
                {
                    cantReason = Translator.Translate("JetPack.JumpReasonRoofed");
                    return false;
                }
                ThingDef chkSKF = DefDatabase<ThingDef>.GetNamed(this.JPSkyFallType, false);
                if (chkSKF == null || !chkSKF.skyfaller.hitRoof)
                {
                    cantReason = TranslatorFormattedStringExtensions.Translate("JetPack.JumpReasonSFNotRPunch", (chkSKF != null) ? GenText.CapitalizeFirst(chkSKF.label) : null);
                    return false;
                }
            }
            if (!GenGrid.Walkable(tCell, pilot.Map))
            {
                cantReason = Translator.Translate("JetPack.JumpReasonNotWalk");
                return false;
            }
            if (DangerUtility.GetDangerFor(tCell, pilot, pilot.Map) == Danger.Deadly)
            {
                cantReason = Translator.Translate("JetPack.JumpReasonDeadly");
                return false;
            }
            float distance = IntVec3Utility.DistanceTo(pilot.Position, tCell);
            if (distance < minJump)
            {
                cantReason = TranslatorFormattedStringExtensions.Translate("JetPack.JumpReasonMinRange", ((int)minJump).ToString());
                return false;
            }
            if (distance > maxJump)
            {
                cantReason = TranslatorFormattedStringExtensions.Translate("JetPack.JumpReasonMaxRange", ((int)maxJump).ToString());
                return false;
            }
            if (fRate <= 0f)
            {
                cantReason = TranslatorFormattedStringExtensions.Translate("JetPack.JumpFuelRateInvalid", fRate.ToString());
                return false;
            }
            float distCanJump;
            if (Settings.UseCarry)
            {
                distCanJump = (float)fuel / fRate * JPWeightUtility.JPCarryFactor(pilot, this.def, this.JPFuelItem);
            }
            else
            {
                distCanJump = (float)fuel / fRate;
            }
            if (distCanJump > maxJump)
            {
                distCanJump = maxJump;
            }
            if (distCanJump < distance)
            {
                cantReason = Translator.Translate("JetPack.JumpNotEnoughfuel");
                return false;
            }
            return true;
        }

        // Token: 0x0600002B RID: 43 RVA: 0x0000302C File Offset: 0x0000122C
        internal void RefuelJetPack(Pawn pilot, ThingWithComps thing)
        {
            List<FloatMenuOption> list = new List<FloatMenuOption>();
            string text = Translator.Translate("JetPack.JPDoNothing");
            list.Add(new FloatMenuOption(text, delegate ()
            {
                this.RefuelJP(pilot, thing, false);
            }, MenuOptionPriority.Default, null, null, 29f, null, null));
            if (this.JPFuelAmount < this.JPFuelMax)
            {
                text = Translator.Translate("JetPack.JPDoRefuel");
                list.Add(new FloatMenuOption(text, delegate ()
                {
                    this.RefuelJP(pilot, thing, true);
                }, MenuOptionPriority.Default, null, null, 29f, null, null));
            }
            if (this.JPFuelAmount > 0)
            {
                text = Translator.Translate("JetPack.JPDropFuel");
                list.Add(new FloatMenuOption(text, delegate ()
                {
                    this.DropFuelJP(pilot, thing);
                }, MenuOptionPriority.Default, null, null, 29f, null, null));
            }
            List<string> fuels = JPUtility.ListFuelTypes(this.def);
            if (fuels.Count > 0)
            {
                for (int i = 0; i < fuels.Count; i++)
                {
                    ThingDef fuelref = DefDatabase<ThingDef>.GetNamed(fuels[i], true);
                    if (fuelref != (thing as JetPackApparel).JPFuelItem)
                    {
                        text = TranslatorFormattedStringExtensions.Translate("JetPack.JPDoChangeFuel", GenText.CapitalizeFirst(fuelref.label));
                        list.Add(new FloatMenuOption(text, delegate ()
                        {
                            this.ChangeFuelJP(pilot, thing, fuelref, this.JPJumpRangeMin);
                        }, MenuOptionPriority.Default, null, null, 29f, null, null));
                    }
                }
            }
            Find.WindowStack.Add(new FloatMenu(list));
        }

        // Token: 0x0600002C RID: 44 RVA: 0x000031D8 File Offset: 0x000013D8
        public void RefuelJP(Pawn pilot, Thing JP, bool Using)
        {
            if (Using)
            {
                if (this.JPComposMentis(pilot, JP, out string Reason))
                {
                    if (this.JPFuelAmount >= this.JPFuelMax)
                    {
                        Messages.Message(TranslatorFormattedStringExtensions.Translate("JetPack.FullyFueled", GenText.CapitalizeFirst(JP.Label)), pilot, MessageTypeDefOf.NeutralEvent, false);
                        SoundStarter.PlayOneShotOnCamera(SoundDefOf.ClickReject, null);
                        return;
                    }
                    JobDef JPRefuel = DefDatabase<JobDef>.GetNamed("JPRefuel", true);
                    this.FindBestJPFuel(pilot, out Thing targ);
                    if (targ != null)
                    {
                        Job job = new Job(JPRefuel, targ);
                        pilot.jobs.TryTakeOrderedJob(job, 0);
                        return;
                    }
                    Messages.Message(TranslatorFormattedStringExtensions.Translate("JetPack.NoFuelFound", GenText.CapitalizeFirst(this.JPFuelItem.label)), pilot, MessageTypeDefOf.NeutralEvent, false);
                    SoundStarter.PlayOneShotOnCamera(SoundDefOf.ClickReject, null);
                    return;
                }
                else
                {
                    Messages.Message(TranslatorFormattedStringExtensions.Translate("JetPack.CantDo", pilot, Reason), pilot, MessageTypeDefOf.NeutralEvent, false);
                    SoundStarter.PlayOneShotOnCamera(SoundDefOf.ClickReject, null);
                }
            }
        }

        // Token: 0x0600002D RID: 45 RVA: 0x000032F0 File Offset: 0x000014F0
        public void ChangeFuelJP(Pawn pilot, Thing JP, ThingDef fueldef, float MinRange)
        {
            if (this.JPFuelItem != fueldef)
            {
                this.RemoveFuel(pilot);
                this.JPFuelAmount = 0;
                this.JPFuelItem = fueldef;
                this.JPFuelRate = JPUtility.GetFuelRate(fueldef);
                this.JPJumpRangeMax = (float)JPUtility.GetJumpRange(pilot, JP.def, fueldef, MinRange);
            }
        }

        // Token: 0x0600002E RID: 46 RVA: 0x0000333D File Offset: 0x0000153D
        public void DropFuelJP(Pawn pilot, Thing JP)
        {
            this.RemoveFuel(pilot);
            this.JPFuelAmount = 0;
        }

        // Token: 0x0600002F RID: 47 RVA: 0x00003350 File Offset: 0x00001550
        public void RemoveFuel(Pawn pilot)
        {
            if (this.JPFuelAmount > 0)
            {
                int amountToDrop = this.JPFuelAmount;
                Thing dropfuel = default;
                while (amountToDrop > 0)
                {
                    Thing oldfuel = ThingMaker.MakeThing(this.JPFuelItem, null);
                    if (oldfuel != null)
                    {
                        int num = this.JPFuelItem.stackLimit;
                        if (amountToDrop <= num)
                        {
                            oldfuel.stackCount = amountToDrop;
                            amountToDrop = 0;
                        }
                        else
                        {
                            oldfuel.stackCount = num;
                            amountToDrop -= num;
                        }
                        GenPlace.TryPlaceThing(oldfuel, pilot.Position, pilot.Map, ThingPlaceMode.Near, out dropfuel, null, null, default);
                    }
                    else
                    {
                        amountToDrop = 0;
                        Log.Message("Couldn't make new fuel item for JP fuel change", false);
                    }
                }
            }
        }

        // Token: 0x06000030 RID: 48 RVA: 0x000033DC File Offset: 0x000015DC
        internal void FindBestJPFuel(Pawn pilot, out Thing targ)
        {
            targ = null;
            if ((pilot?.Map) != null)
            {
                List<Thing> listfuel = pilot?.Map.listerThings.ThingsOfDef(this.JPFuelItem);
                int fuelneeded = this.JPFuelMax - this.JPFuelAmount;
                if (fuelneeded > this.JPFuelItem.stackLimit)
                {
                    fuelneeded = this.JPFuelItem.stackLimit;
                }
                if (listfuel.Count > 0)
                {
                    Thing besttarg = null;
                    float bestpoints = 0f;
                    for (int i = 0; i < listfuel.Count; i++)
                    {
                        Thing targchk = listfuel[i];
                        if (!ForbidUtility.IsForbidden(targchk, pilot) && ((targchk?.Faction) == null || targchk.Faction.IsPlayer) && ReservationUtility.CanReserveAndReach(pilot, targchk, PathEndMode.ClosestTouch, Danger.None, 1, -1, null, false))
                        {
                            float targpoints;
                            if (targchk.stackCount >= fuelneeded)
                            {
                                targpoints = (float)targchk.stackCount / IntVec3Utility.DistanceTo(pilot.Position, targchk.Position);
                            }
                            else
                            {
                                targpoints = (float)targchk.stackCount / (IntVec3Utility.DistanceTo(pilot.Position, targchk.Position) * 2f);
                            }
                            if (targpoints > bestpoints)
                            {
                                besttarg = targchk;
                                bestpoints = targpoints;
                            }
                        }
                    }
                    if (besttarg != null)
                    {
                        targ = besttarg;
                    }
                }
            }
        }

        // Token: 0x06000031 RID: 49 RVA: 0x00003520 File Offset: 0x00001720
        internal bool FlightChecksOK(Pawn pilot, Thing JP, out string checksReason)
        {
            checksReason = "";
            if ((pilot?.Map) == null)
            {
                checksReason = Translator.Translate("JetPack.ChecksLocation");
                return false;
            }
            if (GridsUtility.Roofed(pilot.Position, pilot.Map))
            {
                if (!Settings.RoofPunch)
                {
                    checksReason = Translator.Translate("JetPack.ChecksRoofed");
                    return false;
                }
                ThingDef chkSKF = DefDatabase<ThingDef>.GetNamed(this.JPSkyFallType, false);
                if (chkSKF == null || !chkSKF.skyfaller.hitRoof)
                {
                    checksReason = TranslatorFormattedStringExtensions.Translate("JetPack.ChecksSFNotRPunch", (chkSKF != null) ? GenText.CapitalizeFirst(chkSKF.label) : null);
                    return false;
                }
            }
            if (this.JPFuelAmount < this.JPFuelMin)
            {
                checksReason = Translator.Translate("JetPack.ChecksRefuel") + GenText.CapitalizeFirst(JP.Label);
                return false;
            }
            return true;
        }

        // Token: 0x06000032 RID: 50 RVA: 0x000035FC File Offset: 0x000017FC
        internal bool JPComposMentis(Pawn pilot, Thing JP, out string Reason)
        {
            Reason = "";
            if (pilot == null)
            {
                Reason = Translator.Translate("JetPack.ReasonNotFound");
                return false;
            }
            if (FireUtility.IsBurning(pilot))
            {
                Reason = Translator.Translate("JetPack.ReasonOnFire");
                return false;
            }
            if (pilot.Dead)
            {
                Reason = Translator.Translate("JetPack.ReasonDead");
                return false;
            }
            if (pilot.InMentalState)
            {
                Reason = Translator.Translate("JetPack.ReasonMental");
                return false;
            }
            if (pilot.Downed || pilot.stances.stunner.Stunned)
            {
                Reason = Translator.Translate("JetPack.ReasonIncap");
                return false;
            }
            if (!RestUtility.Awake(pilot) || !pilot.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
            {
                Reason = Translator.Translate("JetPack.ReasonOperate") + GenText.CapitalizeFirst(JP.Label);
                return false;
            }
            return true;
        }

        // Token: 0x06000033 RID: 51 RVA: 0x000036E8 File Offset: 0x000018E8
        public void DoJetPackDebug(Pawn p, Thing t)
        {
            List<FloatMenuOption> list = new List<FloatMenuOption>();
            string text = "Set fuel 100%";
            list.Add(new FloatMenuOption(text, delegate ()
            {
                this.DebugUseJP(p, t, true, 100);
            }, MenuOptionPriority.Default, null, null, 29f, null, null));
            text = "Set fuel 75%";
            list.Add(new FloatMenuOption(text, delegate ()
            {
                this.DebugUseJP(p, t, true, 75);
            }, MenuOptionPriority.Default, null, null, 29f, null, null));
            text = "Set fuel 50%";
            list.Add(new FloatMenuOption(text, delegate ()
            {
                this.DebugUseJP(p, t, true, 50);
            }, MenuOptionPriority.Default, null, null, 29f, null, null));
            text = "Set fuel 25%";
            list.Add(new FloatMenuOption(text, delegate ()
            {
                this.DebugUseJP(p, t, true, 25);
            }, MenuOptionPriority.Default, null, null, 29f, null, null));
            text = "Set fuel 0%";
            list.Add(new FloatMenuOption(text, delegate ()
            {
                this.DebugUseJP(p, t, true, 0);
            }, MenuOptionPriority.Default, null, null, 29f, null, null));
            Find.WindowStack.Add(new FloatMenu(list));
        }

        // Token: 0x06000034 RID: 52 RVA: 0x000037EE File Offset: 0x000019EE
        public void DebugUseJP(Pawn p, Thing t, bool fuelset, int fuellevel)
        {
            if (fuelset)
            {
                this.JPFuelAmount = this.JPFuelMax * fuellevel / 100;
            }
        }

        // Token: 0x04000014 RID: 20
        internal bool debug = true;

        // Token: 0x04000015 RID: 21
        public ThingDef JPFuelItem;

        // Token: 0x04000016 RID: 22
        public int JPFuelAmount;

        // Token: 0x04000017 RID: 23
        public int JPFuelMax;

        // Token: 0x04000018 RID: 24
        public int JPFuelMin;

        // Token: 0x04000019 RID: 25
        public float JPFuelRate;

        // Token: 0x0400001A RID: 26
        public float JPJumpRangeMax;

        // Token: 0x0400001B RID: 27
        public float JPJumpRangeMin;

        // Token: 0x0400001C RID: 28
        public bool JPPilotIsDrafted;

        // Token: 0x0400001D RID: 29
        public string JPSkyFallType;

        // Token: 0x0400001E RID: 30
        public bool JPSlowBurn;

        // Token: 0x0400001F RID: 31
        public int JPSlowBurnTicks;

        // Token: 0x04000020 RID: 32
        public int JPCooldownTicks;

        // Token: 0x04000021 RID: 33
        [NoTranslate]
        private readonly string JPSBTexPath = "Things/Special/JPSlowBurn";
    }
}
