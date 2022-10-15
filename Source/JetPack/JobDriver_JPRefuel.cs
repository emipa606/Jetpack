using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace JetPack;

public class JobDriver_JPRefuel : JobDriver
{
    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        return pawn.Reserve(job.targetA, job);
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        var actor = GetActor();
        this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
        yield return Toils_Reserve.Reserve(TargetIndex.A);
        yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch)
            .FailOnDespawnedNullOrForbidden(TargetIndex.A);
        var refuel = Toils_General.Wait(180);
        refuel.FailOnDespawnedNullOrForbidden(TargetIndex.A);
        refuel.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
        refuel.WithProgressBarToilDelay(TargetIndex.A);
        yield return refuel;
        yield return new Toil
        {
            initAction = delegate
            {
                if (actor != null && actor.apparel.WornApparelCount == 0)
                {
                    //Log.Message("True: obj != null && obj.apparel.WornApparelCount == 0");
                    EndJobWith(JobCondition.Incompletable);
                    return;
                }

                Apparel JetPack = null;
                if (actor != null)
                {
                    var list = actor.apparel.WornApparel;
                    foreach (var jetPack in list)
                    {
                        if (jetPack is not JetPackApparel)
                        {
                            continue;
                        }

                        JetPack = jetPack;
                        break;
                    }
                }

                if (JetPack == null)
                {
                    EndJobWith(JobCondition.Incompletable);
                    return;
                }

                //Log.Message("True: JetPack is JetPackApparel");
                var JPFuel = (JetPack as JetPackApparel).JPFuelAmount;
                var JPMax = (JetPack as JetPackApparel).JPFuelMax;

                if (JPMax - JPFuel <= 0)
                {
                    //Log.Message("True: JPMax - JPFuel <= 0");
                    EndJobWith(JobCondition.Incompletable);
                    return;
                }

                if (TargetThingA.stackCount > JPMax - JPFuel)
                {
                    //Log.Message("True: this.TargetThingA.stackCount > JPMax - JPFuel");
                    (JetPack as JetPackApparel).JPFuelAmount = JPMax;
                    TargetThingA.stackCount -= JPMax - JPFuel;
                    Messages.Message("JetPack.FullyRefueled".Translate(actor.LabelShort), actor,
                        MessageTypeDefOf.NeutralEvent, false);
                    EndJobWith(JobCondition.Succeeded);
                    return;
                }

                //Log.Message("False");
                (JetPack as JetPackApparel).JPFuelAmount = JPFuel + TargetThingA.stackCount;
                Messages.Message(
                    "JetPack.Refueled".Translate(actor.LabelShort.CapitalizeFirst(),
                        TargetThingA.stackCount.ToString(), TargetThingA.stackCount > 1 ? "s" : ""), actor,
                    MessageTypeDefOf.NeutralEvent, false);
                TargetThingA.Destroy();
                EndJobWith(JobCondition.Succeeded);
            }
        };
    }
}