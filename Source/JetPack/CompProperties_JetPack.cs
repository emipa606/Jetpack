using Verse;

namespace JetPack;

public class CompProperties_JetPack : CompProperties
{
    public readonly string JPFuel = "Chemfuel";

    public readonly float JPFuelBurnRate = 1f;

    public readonly int JPFuelMaximum = 150;

    public readonly int JPFuelMinimum = 5;

    public readonly float JPJumpMax = 20f;

    public readonly float JPJumpMin = 5f;

    public readonly string JPSKType = "SFJetPack";

    public int JPFuelLevel;

    public CompProperties_JetPack()
    {
        compClass = typeof(CompJetPack);
    }
}