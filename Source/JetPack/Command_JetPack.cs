using System;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace JetPack;

public class Command_JetPack : Command
{
    public static bool JPRoofPunch = Settings.RoofPunch;

    internal static TargetingParameters targParms = ForJetPacksDestination();

    [NoTranslate] internal static string JPIconPath = "Things/Special/JetPackIcon";

    public Action<IntVec3> action;

    public float JPFRate;

    public int JPFuel;

    public float JPMaxJump;

    public float JPMinJump;

    public string JPSKFStr;

    public Pawn Pilot;

    public override Color IconDrawColor => base.IconDrawColor;

    internal static TargetingParameters ForJetPacksDestination()
    {
        var targetingParameters = new TargetingParameters
        {
            canTargetLocations = true,
            canTargetSelf = false,
            canTargetPawns = false,
            canTargetFires = false,
            canTargetBuildings = false,
            canTargetItems = false,
            validator = x => DropCellFinder.IsGoodDropSpot(x.Cell, x.Map, true, JPRoofPunch)
        };
        return targetingParameters;
    }

    public override void ProcessInput(Event ev)
    {
        base.ProcessInput(ev);
        SoundDefOf.Tick_Tiny.PlayOneShotOnCamera();
        var JPIcon = ContentFinder<Texture2D>.Get(JPIconPath);
        Find.Targeter.BeginTargeting(targParms, delegate(LocalTargetInfo target) { action(target.Cell); }, Pilot,
            null, JPIcon);
    }

    public override void GizmoUpdateOnMouseover()
    {
        if (Find.CurrentMap == null)
        {
            return;
        }

        var MaxRadius = 0f;
        if (JPFRate > 0f)
        {
            MaxRadius = JPFuel / JPFRate;
        }

        if (MaxRadius > JPMaxJump)
        {
            MaxRadius = JPMaxJump;
        }

        if (MaxRadius < JPMinJump)
        {
            MaxRadius = JPMinJump;
        }

        if (MaxRadius > 0f)
        {
            GenDraw.DrawRadiusRing(Pilot.Position, MaxRadius);
        }

        if (JPMinJump > 0f)
        {
            GenDraw.DrawRadiusRing(Pilot.Position, JPMinJump);
        }
    }

    public override bool InheritInteractionsFrom(Gizmo other)
    {
        return false;
    }
}