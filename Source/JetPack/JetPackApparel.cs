using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace JetPack;

public class JetPackApparel : Apparel
{
    [NoTranslate] private readonly string JPSBTexPath = "Things/Special/JPSlowBurn";

    internal bool debug = true;

    public int JPCooldownTicks;

    public int JPFuelAmount;

    public ThingDef JPFuelItem;

    public int JPFuelMax;

    private int JPFuelMin;

    private float JPFuelRate;

    private float JPJumpRangeMax;

    private float JPJumpRangeMin;

    public bool JPPilotIsDrafted;

    private string JPSkyFallType;

    public bool JPSlowBurn;

    private int JPSlowBurnTicks;

    public CompJetPack JetPackComp => GetComp<CompJetPack>();

    private bool JPOnCooldown => JPCooldownTicks > 0;

    public override void PostMake()
    {
        base.PostMake();
        JPFuelItem = DefDatabase<ThingDef>.GetNamed(GetComp<CompJetPack>().Props.JPFuel, false);
        JPFuelAmount = GetComp<CompJetPack>().Props.JPFuelLevel;
        JPFuelMax = GetComp<CompJetPack>().Props.JPFuelMaximum;
        JPFuelMin = GetComp<CompJetPack>().Props.JPFuelMinimum;
        JPFuelRate = GetComp<CompJetPack>().Props.JPFuelBurnRate;
        JPJumpRangeMax = GetComp<CompJetPack>().Props.JPJumpMax;
        JPJumpRangeMin = GetComp<CompJetPack>().Props.JPJumpMin;
        JPSkyFallType = GetComp<CompJetPack>().Props.JPSKType;
        JPPilotIsDrafted = false;
        JPSlowBurn = false;
        JPSlowBurnTicks = 0;
        if (JPFuelItem == null)
        {
            JPFuelItem = ThingDefOf.Chemfuel;
            Log.Message("Warning: Jet pack fuel item not defined, defaulting to chemfuel");
        }
        else
        {
            var pilot = Wearer;
            if (JPFuelItem == DefDatabase<ThingDef>.GetNamed("MSHydrogenPeroxide", false))
            {
                JPJumpRangeMax = JPUtility.GetJumpRange(pilot, def, JPFuelItem, JPJumpRangeMin);
                JPFuelRate = JPUtility.GetFuelRate(JPFuelItem);
            }
            else if (JPFuelItem == DefDatabase<ThingDef>.GetNamed("JPKerosene"))
            {
                JPJumpRangeMax = JPUtility.GetJumpRange(pilot, def, JPFuelItem, JPJumpRangeMin);
                JPFuelRate = JPUtility.GetFuelRate(JPFuelItem);
            }
        }

        if (JPFuelAmount < 0)
        {
            JPFuelAmount = 0;
        }

        if (JPFuelMax < 75)
        {
            JPFuelMax = 75;
        }

        if (JPFuelMax > 150)
        {
            JPFuelMax = 150;
        }

        if (JPFuelMin < 5)
        {
            JPFuelMin = 5;
        }

        if (JPFuelMin > JPFuelMax)
        {
            JPFuelMin = JPFuelMax;
        }

        if (JPFuelRate < 0.1f)
        {
            JPFuelRate = 0.1f;
        }

        if (JPFuelRate > 1f)
        {
            JPFuelRate = 1f;
        }

        if (JPJumpRangeMax > 30f)
        {
            JPJumpRangeMax = 30f;
        }

        if (JPJumpRangeMax < 10f)
        {
            JPJumpRangeMax = 10f;
        }

        if (JPJumpRangeMin < 5f)
        {
            JPJumpRangeMin = 5f;
        }

        if (JPJumpRangeMin > 9f)
        {
            JPJumpRangeMin = 9f;
        }

        if (def.defName != "Apparel_PowArmCGearJetPack")
        {
            if (def.defName != "Apparel_PowArmJetPack")
            {
                if (def.defName != "Apparel_BoosterJetPack")
                {
                    if (JPSkyFallType != null && JPSkyFallType != "SFJetPack")
                    {
                        JPSkyFallType = "SFJetPack";
                    }
                }
                else
                {
                    JPSkyFallType = "SFBoostPack";
                }
            }
            else
            {
                JPSkyFallType = "SFJetPackPowArm";
            }
        }
        else
        {
            JPSkyFallType = "SFJetPackCGear";
        }

        if (JPFuelAmount > JPFuelMax)
        {
            JPFuelAmount = JPFuelMax;
        }
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Defs.Look(ref JPFuelItem, "JPFuelItem");
        Scribe_Values.Look(ref JPFuelAmount, "JPFuelAmount");
        Scribe_Values.Look(ref JPFuelMax, "JPFuelMax", 150);
        Scribe_Values.Look(ref JPFuelMin, "JPFuelMin", 10);
        Scribe_Values.Look(ref JPFuelRate, "JPFuelRate", 1f);
        Scribe_Values.Look(ref JPJumpRangeMax, "JPJumpRangeMax", 25f);
        Scribe_Values.Look(ref JPJumpRangeMin, "JPJumpRangeMin", 10f);
        Scribe_Values.Look(ref JPPilotIsDrafted, "JPPilotIsDrafted");
        Scribe_Values.Look(ref JPSkyFallType, "JPSkyFallType", "SFJetPack");
        Scribe_Values.Look(ref JPSlowBurn, "JPSlowBurn");
        Scribe_Values.Look(ref JPSlowBurnTicks, "JPSlowBurnTicks");
    }

    protected override void Tick()
    {
        base.Tick();
        if (JPCooldownTicks > 0)
        {
            JPCooldownTicks--;
        }

        if (Settings.AllowSlowBurn)
        {
            if (Wearer == null)
            {
                JPSlowBurn = false;
                return;
            }

            if (!JPSlowBurn || !Wearer.Spawned || Wearer.GetPosture() == PawnPosture.LayingInBed ||
                Wearer.GetPosture() == PawnPosture.LayingOnGroundFaceUp ||
                Wearer.GetPosture() == PawnPosture.LayingOnGroundNormal)
            {
                return;
            }

            if (JPSlowBurnTicks > 0)
            {
                JPSlowBurnTicks--;
                return;
            }

            if (JPFuelAmount > 0)
            {
                JPFuelAmount--;
                JPSlowBurnTicks = JPUtility.GetSlowBurnTicks(JPFuelItem);
                return;
            }

            JPSlowBurn = false;
            Messages.Message(
                "JetPack.SBOutOfFuel".Translate(Wearer.LabelShort.CapitalizeFirst(), Label.CapitalizeFirst()),
                Wearer, MessageTypeDefOf.NeutralEvent, false);
        }
        else
        {
            JPSlowBurn = false;
        }
    }


    public override IEnumerable<Gizmo> GetWornGizmos()
    {
        var fuelicon = JPFuelItem.uiIcon;
        if (fuelicon == null)
        {
            ThingDef chkFuelItem = null;
            if (JPFuelItem == null)
            {
                chkFuelItem = DefDatabase<ThingDef>.GetNamed(GetComp<CompJetPack>().Props.JPFuel, false);
            }

            if (chkFuelItem != null && chkFuelItem != JPFuelItem)
            {
                JPFuelItem = chkFuelItem;
                var chkFuelMax = GetComp<CompJetPack>().Props.JPFuelMaximum;
                if (chkFuelMax != JPFuelMax)
                {
                    JPFuelMax = chkFuelMax;
                    if (JPFuelAmount > JPFuelMax)
                    {
                        JPFuelAmount = JPFuelMax;
                    }
                }
            }

            fuelicon = JPFuelItem.uiIcon;
            if (fuelicon == null)
            {
                JPFuelItem = ThingDefOf.Chemfuel;
                fuelicon = JPFuelItem.uiIcon;
                if (fuelicon == null)
                {
                    fuelicon = def.uiIcon;
                    Log.Message("Error: Jet Pack fuel definition not found.");
                }
            }
        }

        var chkSkyFallType = GetComp<CompJetPack>().Props.JPSKType;
        JPSkyFallType = string.IsNullOrEmpty(chkSkyFallType) ? "SFJetPack" : chkSkyFallType;

        var pilot = Wearer;
        JPJumpRangeMax = JPUtility.GetJumpRange(pilot, def, JPFuelItem, JPJumpRangeMin);
        JPFuelRate = JPUtility.GetFuelRate(JPFuelItem);
        if (Wearer is not { IsColonistPlayerControlled: true })
        {
            yield break;
        }

        var wearer = Wearer;
        if (wearer?.Map == null || Wearer.Downed || !Wearer.Spawned || JPUtility.IsInMeleeWithJp(Wearer))
        {
            yield break;
        }

        if (Find.Selector.SingleSelectedThing != Wearer)
        {
            yield break;
        }

        string text = JPOnCooldown
            ? "JetPack.CooldownTicks".Translate(JPCooldownTicks.ToString())
            : "JetPack.JPJump".Translate();

        string desc = "JetPack.JPDesc".Translate(def.label.CapitalizeFirst());
        yield return new Command_JetPack
        {
            defaultLabel = text,
            defaultDesc = desc,
            icon = def.uiIcon,
            Pilot = Wearer,
            JPFuel = JPFuelAmount,
            JPFRate = JPFuelRate,
            JPMaxJump = JPJumpRangeMax,
            JPMinJump = JPJumpRangeMin,
            JPSKFStr = JPSkyFallType,
            action = delegate(IntVec3 cell)
            {
                SoundDefOf.Click.PlayOneShotOnCamera();
                useJetPack(Wearer, this, cell);
            }
        };
        if (Settings.AllowSlowBurn)
        {
            text = "JetPack.JPSBSet".Translate();
            desc = "JetPack.JPSBDesc".Translate();
            yield return new Command_Toggle
            {
                icon = ContentFinder<Texture2D>.Get(JPSBTexPath),
                defaultLabel = text,
                defaultDesc = desc,
                isActive = () => JPSlowBurn,
                toggleAction = delegate { toggleSlowBurn(JPSlowBurn); }
            };
        }

        text = "JetPack.JPLabel".Translate(def.label.CapitalizeFirst(), JPFuelAmount.ToString(),
            JPFuelMax.ToString());
        desc = "JetPack.JPDesc".Translate(def.label.CapitalizeFirst());
        yield return new Command_Action
        {
            defaultLabel = text,
            defaultDesc = desc,
            icon = fuelicon,
            action = delegate
            {
                SoundDefOf.Click.PlayOneShotOnCamera();
                refuelJetPack(Wearer, this);
            }
        };
        if (Prefs.DevMode)
        {
            yield return new Command_Action
            {
                defaultLabel = "Debug Settings",
                defaultDesc = "Debug Commands",
                icon = fuelicon,
                action = delegate
                {
                    SoundDefOf.Click.PlayOneShotOnCamera();
                    doJetPackDebug();
                }
            };
        }
    }

    private void toggleSlowBurn(bool flag)
    {
        JPSlowBurn = !flag;
    }

    private void useJetPack(Pawn pilot, Thing JP, IntVec3 targCell)
    {
        if (!jpComposMentis(pilot, JP, out var Reason))
        {
            Messages.Message("JetPack.CantDo".Translate(pilot, Reason), pilot, MessageTypeDefOf.NeutralEvent,
                false);
            SoundDefOf.ClickReject.PlayOneShotOnCamera();
            return;
        }

        if (!flightChecksOk(pilot, JP, out var ChecksReason))
        {
            Messages.Message("JetPack.ChecksReason".Translate(pilot.LabelShort.CapitalizeFirst(), ChecksReason),
                pilot, MessageTypeDefOf.NeutralEvent, false);
            SoundDefOf.ClickReject.PlayOneShotOnCamera();
            return;
        }

        if (JPUtility.ChkForDisallowed(pilot, out var DAllowReason))
        {
            Messages.Message("JetPack.DAllowReason".Translate(pilot.LabelShort.CapitalizeFirst(), DAllowReason),
                pilot, MessageTypeDefOf.NeutralEvent, false);
            SoundDefOf.ClickReject.PlayOneShotOnCamera();
            return;
        }

        if (!flightCellCheck(pilot, targCell, JPFuelAmount, JPFuelRate, JPJumpRangeMin, JPJumpRangeMax,
                out var JumpReason))
        {
            Messages.Message("JetPack.JumpReason".Translate(JumpReason), pilot, MessageTypeDefOf.NeutralEvent,
                false);
            SoundDefOf.ClickReject.PlayOneShotOnCamera();
            return;
        }

        if (!JPInjury.CheckForExplosion(this))
        {
            doJumpJet(pilot, targCell);
            return;
        }

        JPInjury.DoJPExplosion(pilot, JPFuelAmount, JPFuelItem);
        JPFuelAmount = 0;
    }

    private void doJumpJet(Pawn Pilot, IntVec3 targCell)
    {
        jpRotatePilot(Pilot, targCell, out var angle);
        JPSkyFallType ??= "SFJetPack";

        var JPSF = DefDatabase<ThingDef>.GetNamed(JPSkyFallType, false);
        if (JPSF == null)
        {
            JPSF = DefDatabase<ThingDef>.GetNamed("SFJetPack");
            JPSkyFallType = "SFJetPack";
        }

        var SFspeed = JPSF.skyfaller.speed;
        if (SFspeed <= 0f)
        {
            SFspeed = 1f;
        }

        var distance = Pilot.Position.DistanceTo(targCell);
        var timeToLand = (int)(distance / SFspeed);
        angle += 270f;
        if (angle >= 360f)
        {
            angle -= 360f;
        }

        int fuelUsed;
        if (Settings.UseCarry)
        {
            fuelUsed = (int)Math.Floor(distance * JPFuelRate /
                                       JPWeightUtility.JPCarryFactor(Pilot, def, JPFuelItem));
        }
        else
        {
            fuelUsed = (int)Math.Floor(distance * JPFuelRate);
        }

        JPFuelAmount -= fuelUsed;
        var skyfaller = SkyfallerMaker.SpawnSkyfaller(JPSF, targCell, Pilot.Map);
        skyfaller.ticksToImpact = timeToLand;
        skyfaller.angle = angle;
        JPPilotIsDrafted = Pilot.Drafted;
        Pilot.DeSpawn();
        skyfaller.innerContainer.TryAdd(Pilot, false);
    }

    private static void jpRotatePilot(Pawn pilot, IntVec3 targCell, out float angle)
    {
        angle = pilot.Position.ToVector3().AngleToFlat(targCell.ToVector3());
        var offsetAngle = angle + 90f;
        if (offsetAngle > 360f)
        {
            offsetAngle -= 360f;
        }

        var facing = Rot4.FromAngleFlat(offsetAngle);
        pilot.Rotation = facing;
    }

    private bool flightCellCheck(Pawn pilot, IntVec3 tCell, int fuel, float fRate, float minJump, float maxJump,
        out string cantReason)
    {
        cantReason = "";
        if (!tCell.InBounds(pilot.Map))
        {
            cantReason = "JetPack.JumpReasonInvalid".Translate();
            return false;
        }

        if (JPOnCooldown)
        {
            cantReason = "JetPack.JumpReasonOnCooldown".Translate();
            return false;
        }

        if (!pilot.CanReserve(tCell))
        {
            cantReason = "JetPack.JumpReasonReserved".Translate();
            return false;
        }

        if (tCell.Roofed(pilot.Map))
        {
            if (!Settings.RoofPunch)
            {
                cantReason = "JetPack.JumpReasonRoofed".Translate();
                return false;
            }

            var chkSKF = DefDatabase<ThingDef>.GetNamed(JPSkyFallType, false);
            if (chkSKF == null || !chkSKF.skyfaller.hitRoof)
            {
                cantReason =
                    "JetPack.JumpReasonSFNotRPunch".Translate(
                        chkSKF?.label.CapitalizeFirst());
                return false;
            }
        }

        if (!tCell.Walkable(pilot.Map))
        {
            cantReason = "JetPack.JumpReasonNotWalk".Translate();
            return false;
        }

        if (tCell.GetDangerFor(pilot, pilot.Map) == Danger.Deadly)
        {
            cantReason = "JetPack.JumpReasonDeadly".Translate();
            return false;
        }

        var distance = pilot.Position.DistanceTo(tCell);
        if (distance < minJump)
        {
            cantReason = "JetPack.JumpReasonMinRange".Translate(((int)minJump).ToString());
            return false;
        }

        if (distance > maxJump)
        {
            cantReason = "JetPack.JumpReasonMaxRange".Translate(((int)maxJump).ToString());
            return false;
        }

        if (fRate <= 0f)
        {
            cantReason = "JetPack.JumpFuelRateInvalid".Translate(fRate.ToString());
            return false;
        }

        float distCanJump;
        if (Settings.UseCarry)
        {
            distCanJump = fuel / fRate * JPWeightUtility.JPCarryFactor(pilot, def, JPFuelItem);
        }
        else
        {
            distCanJump = fuel / fRate;
        }

        if (distCanJump > maxJump)
        {
            distCanJump = maxJump;
        }

        if (!(distCanJump < distance))
        {
            return true;
        }

        cantReason = "JetPack.JumpNotEnoughfuel".Translate();
        return false;
    }

    private void refuelJetPack(Pawn pilot, ThingWithComps thing)
    {
        var list = new List<FloatMenuOption>();
        string text = "JetPack.JPDoNothing".Translate();
        list.Add(new FloatMenuOption(text, delegate { refuelJp(pilot, thing, false); }, MenuOptionPriority.Default,
            null, null, 29f));
        if (JPFuelAmount < JPFuelMax)
        {
            text = "JetPack.JPDoRefuel".Translate();
            list.Add(new FloatMenuOption(text, delegate { refuelJp(pilot, thing, true); },
                MenuOptionPriority.Default, null, null, 29f));
        }

        if (JPFuelAmount > 0)
        {
            text = "JetPack.JPDropFuel".Translate();
            list.Add(new FloatMenuOption(text, delegate { dropFuelJp(pilot); }, MenuOptionPriority.Default,
                null, null, 29f));
        }

        var fuels = JPUtility.ListFuelTypes(def);
        if (fuels.Count > 0)
        {
            foreach (var defName in fuels)
            {
                var fuelref = DefDatabase<ThingDef>.GetNamed(defName);
                if (fuelref == (thing as JetPackApparel)?.JPFuelItem)
                {
                    continue;
                }

                text = "JetPack.JPDoChangeFuel".Translate(fuelref.label.CapitalizeFirst());
                list.Add(new FloatMenuOption(text,
                    delegate { changeFuelJp(pilot, thing, fuelref, JPJumpRangeMin); },
                    MenuOptionPriority.Default, null, null, 29f));
            }
        }

        Find.WindowStack.Add(new FloatMenu(list));
    }

    private void refuelJp(Pawn pilot, Thing JP, bool Using)
    {
        if (!Using)
        {
            return;
        }

        if (jpComposMentis(pilot, JP, out var Reason))
        {
            if (JPFuelAmount >= JPFuelMax)
            {
                Messages.Message("JetPack.FullyFueled".Translate(JP.Label.CapitalizeFirst()), pilot,
                    MessageTypeDefOf.NeutralEvent, false);
                SoundDefOf.ClickReject.PlayOneShotOnCamera();
                return;
            }

            var JPRefuel = DefDatabase<JobDef>.GetNamed("JPRefuel");
            findBestJpFuel(pilot, out var targ);
            if (targ != null)
            {
                var job = new Job(JPRefuel, targ);
                pilot.jobs.TryTakeOrderedJob(job, 0);
                return;
            }

            Messages.Message("JetPack.NoFuelFound".Translate(JPFuelItem.label.CapitalizeFirst()), pilot,
                MessageTypeDefOf.NeutralEvent, false);
            SoundDefOf.ClickReject.PlayOneShotOnCamera();
        }
        else
        {
            Messages.Message("JetPack.CantDo".Translate(pilot, Reason), pilot, MessageTypeDefOf.NeutralEvent,
                false);
            SoundDefOf.ClickReject.PlayOneShotOnCamera();
        }
    }

    private void changeFuelJp(Pawn pilot, Thing JP, ThingDef fueldef, float MinRange)
    {
        if (JPFuelItem == fueldef)
        {
            return;
        }

        removeFuel(pilot);
        JPFuelAmount = 0;
        JPFuelItem = fueldef;
        JPFuelRate = JPUtility.GetFuelRate(fueldef);
        JPJumpRangeMax = JPUtility.GetJumpRange(pilot, JP.def, fueldef, MinRange);
    }

    private void dropFuelJp(Pawn pilot)
    {
        removeFuel(pilot);
        JPFuelAmount = 0;
    }

    private void removeFuel(Pawn pilot)
    {
        if (JPFuelAmount <= 0)
        {
            return;
        }

        var amountToDrop = JPFuelAmount;
        while (amountToDrop > 0)
        {
            var oldfuel = ThingMaker.MakeThing(JPFuelItem);
            if (oldfuel != null)
            {
                var num = JPFuelItem.stackLimit;
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

                GenPlace.TryPlaceThing(oldfuel, pilot.Position, pilot.Map, ThingPlaceMode.Near, out _);
            }
            else
            {
                amountToDrop = 0;
                Log.Message("Couldn't make new fuel item for JP fuel change");
            }
        }
    }

    private void findBestJpFuel(Pawn pilot, out Thing targ)
    {
        targ = null;
        if (pilot?.Map == null)
        {
            return;
        }

        var listfuel = pilot.Map.listerThings.ThingsOfDef(JPFuelItem);
        var fuelneeded = JPFuelMax - JPFuelAmount;
        if (fuelneeded > JPFuelItem.stackLimit)
        {
            fuelneeded = JPFuelItem.stackLimit;
        }

        if (listfuel.Count <= 0)
        {
            return;
        }

        Thing besttarg = null;
        var bestpoints = 0f;
        foreach (var targchk in listfuel)
        {
            if (targchk.IsForbidden(pilot) || targchk?.Faction is { IsPlayer: false } ||
                !pilot.CanReserveAndReach(targchk, PathEndMode.ClosestTouch, Danger.None))
            {
                continue;
            }

            float targpoints;
            if (targchk == null)
            {
                continue;
            }

            if (targchk.stackCount >= fuelneeded)
            {
                targpoints = targchk.stackCount / pilot.Position.DistanceTo(targchk.Position);
            }
            else
            {
                targpoints = targchk.stackCount / (pilot.Position.DistanceTo(targchk.Position) * 2f);
            }

            if (!(targpoints > bestpoints))
            {
                continue;
            }

            besttarg = targchk;
            bestpoints = targpoints;
        }

        if (besttarg != null)
        {
            targ = besttarg;
        }
    }

    private bool flightChecksOk(Pawn pilot, Thing JP, out string checksReason)
    {
        checksReason = "";
        if (pilot?.Map == null)
        {
            checksReason = "JetPack.ChecksLocation".Translate();
            return false;
        }

        if (pilot.Position.Roofed(pilot.Map))
        {
            if (!Settings.RoofPunch)
            {
                checksReason = "JetPack.ChecksRoofed".Translate();
                return false;
            }

            var chkSKF = DefDatabase<ThingDef>.GetNamed(JPSkyFallType, false);
            if (chkSKF == null || !chkSKF.skyfaller.hitRoof)
            {
                checksReason =
                    "JetPack.ChecksSFNotRPunch".Translate(chkSKF?.label.CapitalizeFirst());
                return false;
            }
        }

        if (JPFuelAmount >= JPFuelMin)
        {
            return true;
        }

        checksReason = "JetPack.ChecksRefuel".Translate() + JP.Label.CapitalizeFirst();
        return false;
    }

    private static bool jpComposMentis(Pawn pilot, Thing JP, out string Reason)
    {
        Reason = "";
        if (pilot == null)
        {
            Reason = "JetPack.ReasonNotFound".Translate();
            return false;
        }

        if (pilot.IsBurning())
        {
            Reason = "JetPack.ReasonOnFire".Translate();
            return false;
        }

        if (pilot.Dead)
        {
            Reason = "JetPack.ReasonDead".Translate();
            return false;
        }

        if (pilot.InMentalState)
        {
            Reason = "JetPack.ReasonMental".Translate();
            return false;
        }

        if (pilot.Downed || pilot.stances.stunner.Stunned)
        {
            Reason = "JetPack.ReasonIncap".Translate();
            return false;
        }

        if (pilot.Awake() && pilot.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
        {
            return true;
        }

        Reason = "JetPack.ReasonOperate".Translate() + JP.Label.CapitalizeFirst();
        return false;
    }

    private void doJetPackDebug()
    {
        var list = new List<FloatMenuOption>();
        var text = "Set fuel 100%";
        list.Add(new FloatMenuOption(text, delegate { debugUseJp(true, 100); }, MenuOptionPriority.Default,
            null, null, 29f));
        text = "Set fuel 75%";
        list.Add(new FloatMenuOption(text, delegate { debugUseJp(true, 75); }, MenuOptionPriority.Default,
            null, null, 29f));
        text = "Set fuel 50%";
        list.Add(new FloatMenuOption(text, delegate { debugUseJp(true, 50); }, MenuOptionPriority.Default,
            null, null, 29f));
        text = "Set fuel 25%";
        list.Add(new FloatMenuOption(text, delegate { debugUseJp(true, 25); }, MenuOptionPriority.Default,
            null, null, 29f));
        text = "Set fuel 0%";
        list.Add(new FloatMenuOption(text, delegate { debugUseJp(true, 0); }, MenuOptionPriority.Default,
            null, null, 29f));
        Find.WindowStack.Add(new FloatMenu(list));
    }

    private void debugUseJp(bool fuelset, int fuellevel)
    {
        if (fuelset)
        {
            JPFuelAmount = JPFuelMax * fuellevel / 100;
        }
    }
}