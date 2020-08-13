using System;
using Verse;

namespace JetPack
{
    // Token: 0x02000008 RID: 8
    public class CompProperties_JetPack : CompProperties
    {
        // Token: 0x06000015 RID: 21 RVA: 0x0000245C File Offset: 0x0000065C
        public CompProperties_JetPack()
        {
            this.compClass = typeof(CompJetPack);
        }

        // Token: 0x0400000B RID: 11
        public int JPFuelLevel;

        // Token: 0x0400000C RID: 12
        public string JPFuel = "Chemfuel";

        // Token: 0x0400000D RID: 13
        public int JPFuelMaximum = 150;

        // Token: 0x0400000E RID: 14
        public int JPFuelMinimum = 5;

        // Token: 0x0400000F RID: 15
        public float JPFuelBurnRate = 1f;

        // Token: 0x04000010 RID: 16
        public float JPJumpMax = 20f;

        // Token: 0x04000011 RID: 17
        public float JPJumpMin = 5f;

        // Token: 0x04000012 RID: 18
        public string JPSKType = "SFJetPack";
    }
}
