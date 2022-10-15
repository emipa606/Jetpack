using Verse;

namespace JetPack;

public class CompProperties_JetPack : CompProperties
{
    public string JPFuel = "Chemfuel";

    public float JPFuelBurnRate = 1f;

    public int JPFuelLevel;

    public int JPFuelMaximum = 150;

    public int JPFuelMinimum = 5;

    public float JPJumpMax = 20f;

    public float JPJumpMin = 5f;

    public string JPSKType = "SFJetPack";

    public CompProperties_JetPack()
    {
        compClass = typeof(CompJetPack);
    }
}