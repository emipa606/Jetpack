using RimWorld;
using Verse;
using Verse.AI;

namespace JetPack
{
    // Token: 0x02000018 RID: 24
    public class ThinkNode_CanRefuelJP : ThinkNode_Conditional
    {
        // Token: 0x0600007D RID: 125 RVA: 0x00005AF8 File Offset: 0x00003CF8
        protected override bool Satisfied(Pawn pawn)
        {
            return Settings.DoAutoRefuel && pawn.IsColonistPlayerControlled &&
                   pawn.health.capacities.CapableOf(PawnCapacityDefOf.Moving) && !pawn.Downed && !pawn.IsBurning() &&
                   !pawn.InMentalState && !pawn.Drafted && pawn.Awake() && !HealthAIUtility.ShouldSeekMedicalRest(pawn);
        }
    }
}