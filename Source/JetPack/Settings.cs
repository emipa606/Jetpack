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

    public void DoWindowContents(Rect canvas)
    {
        var listing_Standard = new Listing_Standard
        {
            ColumnWidth = canvas.width
        };
        listing_Standard.Begin(canvas);
        listing_Standard.Gap(3f);
        var gap = 3f;
        listing_Standard.CheckboxLabeled("JetPack.RoofPunch".Translate(), ref RoofPunch);
        listing_Standard.Gap(gap);
        listing_Standard.CheckboxLabeled("JetPack.AllowFire".Translate(), ref AllowFire);
        listing_Standard.Gap(gap);
        Text.Font = GameFont.Tiny;
        listing_Standard.Label("          " + "JetPack.ResetTip".Translate());
        Text.Font = GameFont.Small;
        listing_Standard.Gap(gap);
        listing_Standard.CheckboxLabeled("JetPack.DoAutoRefuel".Translate(), ref DoAutoRefuel);
        listing_Standard.Gap(gap);
        listing_Standard.Label("JetPack.RefuelPCT".Translate() + "  " + RefuelPCT);
        checked
        {
            RefuelPCT = (int)listing_Standard.Slider(RefuelPCT, 0f, 75f);
            listing_Standard.Gap(gap);
            listing_Standard.Label("JetPack.CooldownTime".Translate() + "  " + CooldownTime);
            CooldownTime = (int)listing_Standard.Slider(CooldownTime, 0f, 10f);
            listing_Standard.Gap(gap);
            listing_Standard.CheckboxLabeled("JetPack.AllowSlowBurn".Translate(), ref AllowSlowBurn);
            listing_Standard.Gap(gap);
            listing_Standard.CheckboxLabeled("JetPack.ApplyDFA".Translate(), ref ApplyDFA);
            listing_Standard.Gap(gap);
            listing_Standard.CheckboxLabeled("JetPack.ApplyDFASplash".Translate(), ref ApplyDFASplash);
            listing_Standard.Gap(gap);
            listing_Standard.Label("JetPack.DFASplashFactor".Translate() + "  " + (int)DFASplashFactor);
            DFASplashFactor = (int)listing_Standard.Slider((int)DFASplashFactor, 25f, 75f);
            listing_Standard.Gap(gap);
            listing_Standard.CheckboxLabeled("JetPack.AllowBoom".Translate(), ref AllowBoom);
            listing_Standard.Gap(gap);
            listing_Standard.CheckboxLabeled("JetPack.AllowWMD".Translate(), ref AllowWMD);
            listing_Standard.Gap(gap);
            listing_Standard.CheckboxLabeled("JetPack.AllowHVY".Translate(), ref AllowHVY);
            listing_Standard.Gap(gap);
            listing_Standard.CheckboxLabeled("JetPack.UseCarry".Translate(), ref UseCarry);
            listing_Standard.Gap(gap);
        }

        if (Controller.currentVersion != null)
        {
            listing_Standard.Gap();
            GUI.contentColor = Color.gray;
            listing_Standard.Label("JetPack.ModVersion".Translate(Controller.currentVersion));
            GUI.contentColor = Color.white;
        }

        listing_Standard.End();
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