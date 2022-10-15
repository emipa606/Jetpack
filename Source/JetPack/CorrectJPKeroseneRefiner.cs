using Verse;

namespace JetPack;

public static class CorrectJPKeroseneRefiner
{
    public static bool IsProblematicItem(Building b)
    {
        return b.def.defName == "Frame_JPKeroseneRefinery";
    }

    public static bool RFLoaded()
    {
        return ModLister.HasActiveModWithName("Rimefeller");
    }
}