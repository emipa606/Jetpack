using UnityEngine;
using Verse;

namespace JetPack
{
    // Token: 0x02000016 RID: 22
    public class Settings : ModSettings
    {
        // Token: 0x04000036 RID: 54
        public static bool DoAutoRefuel;

        // Token: 0x04000037 RID: 55
        public static int RefuelPCT = 50;

        // Token: 0x04000038 RID: 56
        public static int CooldownTime;

        // Token: 0x04000039 RID: 57
        public static bool AllowSlowBurn;

        // Token: 0x0400003A RID: 58
        public static bool RoofPunch;

        // Token: 0x0400003B RID: 59
        public static bool ApplyDFA;

        // Token: 0x0400003C RID: 60
        public static bool ApplyDFASplash;

        // Token: 0x0400003D RID: 61
        public static float DFASplashFactor = 50f;

        // Token: 0x0400003E RID: 62
        public static bool AllowBoom;

        // Token: 0x0400003F RID: 63
        public static bool AllowWMD;

        // Token: 0x04000040 RID: 64
        public static bool AllowHVY;

        // Token: 0x04000041 RID: 65
        public static bool AllowFire;

        // Token: 0x04000042 RID: 66
        public static bool UseCarry;

        // Token: 0x06000078 RID: 120 RVA: 0x000056C0 File Offset: 0x000038C0
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
                RefuelPCT = (int) listing_Standard.Slider(RefuelPCT, 0f, 75f);
                listing_Standard.Gap(gap);
                listing_Standard.Label("JetPack.CooldownTime".Translate() + "  " + CooldownTime);
                CooldownTime = (int) listing_Standard.Slider(CooldownTime, 0f, 10f);
                listing_Standard.Gap(gap);
                listing_Standard.CheckboxLabeled("JetPack.AllowSlowBurn".Translate(), ref AllowSlowBurn);
                listing_Standard.Gap(gap);
                listing_Standard.CheckboxLabeled("JetPack.ApplyDFA".Translate(), ref ApplyDFA);
                listing_Standard.Gap(gap);
                listing_Standard.CheckboxLabeled("JetPack.ApplyDFASplash".Translate(), ref ApplyDFASplash);
                listing_Standard.Gap(gap);
                listing_Standard.Label("JetPack.DFASplashFactor".Translate() + "  " + (int) DFASplashFactor);
                DFASplashFactor = (int) listing_Standard.Slider((int) DFASplashFactor, 25f, 75f);
                listing_Standard.Gap(gap);
                listing_Standard.CheckboxLabeled("JetPack.AllowBoom".Translate(), ref AllowBoom);
                listing_Standard.Gap(gap);
                listing_Standard.CheckboxLabeled("JetPack.AllowWMD".Translate(), ref AllowWMD);
                listing_Standard.Gap(gap);
                listing_Standard.CheckboxLabeled("JetPack.AllowHVY".Translate(), ref AllowHVY);
                listing_Standard.Gap(gap);
                listing_Standard.CheckboxLabeled("JetPack.UseCarry".Translate(), ref UseCarry);
                listing_Standard.Gap(gap);
                listing_Standard.End();
            }
        }

        // Token: 0x06000079 RID: 121 RVA: 0x00005990 File Offset: 0x00003B90
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
}