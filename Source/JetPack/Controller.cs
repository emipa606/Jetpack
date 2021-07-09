using UnityEngine;
using Verse;

namespace JetPack
{
    // Token: 0x02000009 RID: 9
    public class Controller : Mod
    {
        // Token: 0x04000013 RID: 19
        public static Settings Settings;

        // Token: 0x06000018 RID: 24 RVA: 0x000024E6 File Offset: 0x000006E6
        public Controller(ModContentPack content) : base(content)
        {
            Settings = GetSettings<Settings>();
        }

        // Token: 0x06000016 RID: 22 RVA: 0x000024C8 File Offset: 0x000006C8
        public override string SettingsCategory()
        {
            return "JetPack.Name".Translate();
        }

        // Token: 0x06000017 RID: 23 RVA: 0x000024D9 File Offset: 0x000006D9
        public override void DoSettingsWindowContents(Rect canvas)
        {
            Settings.DoWindowContents(canvas);
        }
    }
}