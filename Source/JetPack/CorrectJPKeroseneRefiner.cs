using Verse;

namespace JetPack
{
    // Token: 0x0200000A RID: 10
    public static class CorrectJPKeroseneRefiner
    {
        // Token: 0x06000019 RID: 25 RVA: 0x000024FA File Offset: 0x000006FA
        public static bool IsProblematicItem(Building b)
        {
            return b.def.defName == "Frame_JPKeroseneRefinery";
        }

        // Token: 0x0600001A RID: 26 RVA: 0x00002516 File Offset: 0x00000716
        public static bool RFLoaded()
        {
            return ModLister.HasActiveModWithName("Rimefeller");
        }
    }
}