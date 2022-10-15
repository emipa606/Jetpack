using RimWorld;
using Verse;
using Verse.AI;

namespace JetPack;

public class ThinkNode_CanRefuelJP : ThinkNode_Conditional
{
    protected override bool Satisfied(Pawn pawn)
    {
        return Settings.DoAutoRefuel && pawn.IsColonistPlayerControlled &&
               pawn.health.capacities.CapableOf(PawnCapacityDefOf.Moving) && !pawn.Downed && !pawn.IsBurning() &&
               !pawn.InMentalState && !pawn.Drafted && pawn.Awake() && !HealthAIUtility.ShouldSeekMedicalRest(pawn);
    }
}