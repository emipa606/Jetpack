using System;
using UnityEngine;
using Verse;

namespace JetPack
{
    // Token: 0x02000016 RID: 22
    public class Settings : ModSettings
    {
        // Token: 0x06000078 RID: 120 RVA: 0x000056C0 File Offset: 0x000038C0
        public void DoWindowContents(Rect canvas)
        {
            Listing_Standard listing_Standard = new Listing_Standard
            {
                ColumnWidth = canvas.width
            };
            listing_Standard.Begin(canvas);
            listing_Standard.Gap(3f);
            float gap = 3f;
            listing_Standard.CheckboxLabeled(Translator.Translate("JetPack.RoofPunch"), ref Settings.RoofPunch, null);
            listing_Standard.Gap(gap);
            listing_Standard.CheckboxLabeled(Translator.Translate("JetPack.AllowFire"), ref Settings.AllowFire, null);
            listing_Standard.Gap(gap);
            Text.Font = GameFont.Tiny;
            listing_Standard.Label("          " + Translator.Translate("JetPack.ResetTip"), -1f, null);
            Text.Font = GameFont.Small;
            listing_Standard.Gap(gap);
            listing_Standard.CheckboxLabeled(Translator.Translate("JetPack.DoAutoRefuel"), ref Settings.DoAutoRefuel, null);
            listing_Standard.Gap(gap);
            listing_Standard.Label(Translator.Translate("JetPack.RefuelPCT") + "  " + Settings.RefuelPCT, -1f, null);
            checked
            {
                Settings.RefuelPCT = (int)listing_Standard.Slider((float)Settings.RefuelPCT, 0f, 75f);
                listing_Standard.Gap(gap);
                listing_Standard.Label(Translator.Translate("JetPack.CooldownTime") + "  " + Settings.CooldownTime, -1f, null);
                Settings.CooldownTime = (int)listing_Standard.Slider((float)Settings.CooldownTime, 0f, 10f);
                listing_Standard.Gap(gap);
                listing_Standard.CheckboxLabeled(Translator.Translate("JetPack.AllowSlowBurn"), ref Settings.AllowSlowBurn, null);
                listing_Standard.Gap(gap);
                listing_Standard.CheckboxLabeled(Translator.Translate("JetPack.ApplyDFA"), ref Settings.ApplyDFA, null);
                listing_Standard.Gap(gap);
                listing_Standard.CheckboxLabeled(Translator.Translate("JetPack.ApplyDFASplash"), ref Settings.ApplyDFASplash, null);
                listing_Standard.Gap(gap);
                listing_Standard.Label(Translator.Translate("JetPack.DFASplashFactor") + "  " + (int)Settings.DFASplashFactor, -1f, null);
                Settings.DFASplashFactor = (float)((int)listing_Standard.Slider((float)((int)Settings.DFASplashFactor), 25f, 75f));
                listing_Standard.Gap(gap);
                listing_Standard.CheckboxLabeled(Translator.Translate("JetPack.AllowBoom"), ref Settings.AllowBoom, null);
                listing_Standard.Gap(gap);
                listing_Standard.CheckboxLabeled(Translator.Translate("JetPack.AllowWMD"), ref Settings.AllowWMD, null);
                listing_Standard.Gap(gap);
                listing_Standard.CheckboxLabeled(Translator.Translate("JetPack.AllowHVY"), ref Settings.AllowHVY, null);
                listing_Standard.Gap(gap);
                listing_Standard.CheckboxLabeled(Translator.Translate("JetPack.UseCarry"), ref Settings.UseCarry, null);
                listing_Standard.Gap(gap);
                listing_Standard.End();
            }
        }

        // Token: 0x06000079 RID: 121 RVA: 0x00005990 File Offset: 0x00003B90
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<bool>(ref Settings.DoAutoRefuel, "DoAutoRefuel", false, false);
            Scribe_Values.Look<int>(ref Settings.RefuelPCT, "RefuelPCT", 50, false);
            Scribe_Values.Look<int>(ref Settings.CooldownTime, "CooldownTime", 0, false);
            Scribe_Values.Look<bool>(ref Settings.AllowSlowBurn, "AllowSlowBurn", false, false);
            Scribe_Values.Look<bool>(ref Settings.RoofPunch, "RoofPunch", false, false);
            Scribe_Values.Look<bool>(ref Settings.ApplyDFA, "ApplyDFA", false, false);
            Scribe_Values.Look<bool>(ref Settings.ApplyDFASplash, "ApplyDFASplash", false, false);
            Scribe_Values.Look<float>(ref Settings.DFASplashFactor, "DFASplashFactor", 50f, false);
            Scribe_Values.Look<bool>(ref Settings.AllowBoom, "AllowBoom", false, false);
            Scribe_Values.Look<bool>(ref Settings.AllowWMD, "AllowWMD", false, false);
            Scribe_Values.Look<bool>(ref Settings.AllowHVY, "AllowHVY", false, false);
            Scribe_Values.Look<bool>(ref Settings.AllowFire, "AllowFire", false, false);
            Scribe_Values.Look<bool>(ref Settings.UseCarry, "UseCarry", false, false);
        }

        // Token: 0x04000036 RID: 54
        public static bool DoAutoRefuel = false;

        // Token: 0x04000037 RID: 55
        public static int RefuelPCT = 50;

        // Token: 0x04000038 RID: 56
        public static int CooldownTime = 0;

        // Token: 0x04000039 RID: 57
        public static bool AllowSlowBurn = false;

        // Token: 0x0400003A RID: 58
        public static bool RoofPunch = false;

        // Token: 0x0400003B RID: 59
        public static bool ApplyDFA = false;

        // Token: 0x0400003C RID: 60
        public static bool ApplyDFASplash = false;

        // Token: 0x0400003D RID: 61
        public static float DFASplashFactor = 50f;

        // Token: 0x0400003E RID: 62
        public static bool AllowBoom = false;

        // Token: 0x0400003F RID: 63
        public static bool AllowWMD = false;

        // Token: 0x04000040 RID: 64
        public static bool AllowHVY = false;

        // Token: 0x04000041 RID: 65
        public static bool AllowFire = false;

        // Token: 0x04000042 RID: 66
        public static bool UseCarry = false;
    }
}
