using UnityEngine;
using Verse;

namespace JetPack;

public class Settings : ModSettings
{
    public static bool DoAutoRefuel;

    public static int RefuelPCT = 50;

    public static int CooldownTime;

    public static bool AllowSlowBurn;

    public static bool RoofPunch;

    public static bool ApplyDFA;

    public static bool ApplyDFASplash;

    public static float DFASplashFactor = 50f;

    public static bool AllowBoom;

    public static bool AllowWMD;

    public static bool AllowHVY;

    public static bool AllowFire;

    public static bool UseCarry;

    public static void DoWindowContents(Rect canvas)
    {
        var listingStandard = new Listing_Standard
        {
            ColumnWidth = canvas.width
        };
        listingStandard.Begin(canvas);
        listingStandard.Gap(3f);
        var gap = 3f;
        listingStandard.CheckboxLabeled("JetPack.RoofPunch".Translate(), ref RoofPunch);
        listingStandard.Gap(gap);
        listingStandard.CheckboxLabeled("JetPack.AllowFire".Translate(), ref AllowFire);
        listingStandard.Gap(gap);
        Text.Font = GameFont.Tiny;
        listingStandard.Label("          " + "JetPack.ResetTip".Translate());
        Text.Font = GameFont.Small;
        listingStandard.Gap(gap);
        listingStandard.CheckboxLabeled("JetPack.DoAutoRefuel".Translate(), ref DoAutoRefuel);
        listingStandard.Gap(gap);
        listingStandard.Label("JetPack.RefuelPCT".Translate() + "  " + RefuelPCT);
        checked
        {
            RefuelPCT = (int)listingStandard.Slider(RefuelPCT, 0f, 75f);
            listingStandard.Gap(gap);
            listingStandard.Label("JetPack.CooldownTime".Translate() + "  " + CooldownTime);
            CooldownTime = (int)listingStandard.Slider(CooldownTime, 0f, 10f);
            listingStandard.Gap(gap);
            listingStandard.CheckboxLabeled("JetPack.AllowSlowBurn".Translate(), ref AllowSlowBurn);
            listingStandard.Gap(gap);
            listingStandard.CheckboxLabeled("JetPack.ApplyDFA".Translate(), ref ApplyDFA);
            listingStandard.Gap(gap);
            listingStandard.CheckboxLabeled("JetPack.ApplyDFASplash".Translate(), ref ApplyDFASplash);
            listingStandard.Gap(gap);
            listingStandard.Label("JetPack.DFASplashFactor".Translate() + "  " + (int)DFASplashFactor);
            DFASplashFactor = (int)listingStandard.Slider((int)DFASplashFactor, 25f, 75f);
            listingStandard.Gap(gap);
            listingStandard.CheckboxLabeled("JetPack.AllowBoom".Translate(), ref AllowBoom);
            listingStandard.Gap(gap);
            listingStandard.CheckboxLabeled("JetPack.AllowWMD".Translate(), ref AllowWMD);
            listingStandard.Gap(gap);
            listingStandard.CheckboxLabeled("JetPack.AllowHVY".Translate(), ref AllowHVY);
            listingStandard.Gap(gap);
            listingStandard.CheckboxLabeled("JetPack.UseCarry".Translate(), ref UseCarry);
            listingStandard.Gap(gap);
        }

        if (Controller.CurrentVersion != null)
        {
            listingStandard.Gap();
            GUI.contentColor = Color.gray;
            listingStandard.Label("JetPack.ModVersion".Translate(Controller.CurrentVersion));
            GUI.contentColor = Color.white;
        }

        listingStandard.End();
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref DoAutoRefuel, "DoAutoRefuel");
        Scribe_Values.Look(ref RefuelPCT, "RefuelPCT", 50);
        Scribe_Values.Look(ref CooldownTime, "CooldownTime");
        Scribe_Values.Look(ref AllowSlowBurn, "AllowSlowBurn");
        Scribe_Values.Look(ref RoofPunch, "RoofPunch");
        Scribe_Values.Look(ref ApplyDFA, "ApplyDFA");
        Scribe_Values.Look(ref ApplyDFASplash, "ApplyDFASplash");
        Scribe_Values.Look(ref DFASplashFactor, "DFASplashFactor", 50f);
        Scribe_Values.Look(ref AllowBoom, "AllowBoom");
        Scribe_Values.Look(ref AllowWMD, "AllowWMD");
        Scribe_Values.Look(ref AllowHVY, "AllowHVY");
        Scribe_Values.Look(ref AllowFire, "AllowFire");
        Scribe_Values.Look(ref UseCarry, "UseCarry");
    }
}