using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace JetPack
{
    // Token: 0x0200000E RID: 14
    public class JobDriver_JPRefuel : JobDriver
    {
        // Token: 0x0600003B RID: 59 RVA: 0x00003884 File Offset: 0x00001A84
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return ReservationUtility.Reserve(this.pawn, this.job.targetA, this.job, 1, -1, null, true);
        }

        // Token: 0x0600003C RID: 60 RVA: 0x000038A6 File Offset: 0x00001AA6
        protected override IEnumerable<Toil> MakeNewToils()
        {
            Pawn actor = base.GetActor();
            ToilFailConditions.FailOnDespawnedNullOrForbidden<JobDriver_JPRefuel>(this, TargetIndex.A);
            yield return Toils_Reserve.Reserve(TargetIndex.A, 1, -1, null);
            yield return ToilFailConditions.FailOnDespawnedNullOrForbidden<Toil>(Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch), TargetIndex.A);
            Toil refuel = Toils_General.Wait(180, 0);
            ToilFailConditions.FailOnDespawnedNullOrForbidden<Toil>(refuel, TargetIndex.A);
            ToilFailConditions.FailOnCannotTouch<Toil>(refuel, TargetIndex.A, PathEndMode.Touch);
            ToilEffects.WithProgressBarToilDelay(refuel, TargetIndex.A, false, -0.5f);
            yield return refuel;
            yield return new Toil
            {
                initAction = delegate ()
                {
                    int JPFuel = 0;
                    int JPMax = 0;
                    Pawn obj = actor;
                    if (obj != null && obj.apparel.WornApparelCount > 0)
                    {
                        this.EndJobWith(JobCondition.Incompletable);
                        return;
                    }
                    Apparel JetPack = null;
                    List<Apparel> list = actor.apparel.WornApparel;
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (list[i] is JetPackApparel)
                        {
                            JetPack = list[i];
                            break;
                        }
                    }
                    if (JetPack == null)
                    {
                        this.EndJobWith(JobCondition.Incompletable);
                        return;
                    }
                    if (JetPack is JetPackApparel)
                    {
                        JPFuel = (JetPack as JetPackApparel).JPFuelAmount;
                        JPMax = (JetPack as JetPackApparel).JPFuelMax;
                    }
                    if (JPMax - JPFuel <= 0)
                    {
                        this.EndJobWith(JobCondition.Incompletable);
                        return;
                    }
                    if (this.TargetThingA.stackCount > JPMax - JPFuel)
                    {
                        (JetPack as JetPackApparel).JPFuelAmount = JPMax;
                        this.TargetThingA.stackCount -= JPMax - JPFuel;
                        Messages.Message(TranslatorFormattedStringExtensions.Translate("JetPack.FullyRefueled", actor.LabelShort), actor, MessageTypeDefOf.NeutralEvent, false);
                        this.EndJobWith(JobCondition.Succeeded);
                        return;
                    }
                    (JetPack as JetPackApparel).JPFuelAmount = JPFuel + this.TargetThingA.stackCount;
                    Messages.Message(TranslatorFormattedStringExtensions.Translate("JetPack.Refueled", GenText.CapitalizeFirst(actor.LabelShort), this.TargetThingA.stackCount.ToString(), (this.TargetThingA.stackCount > 1) ? "s" : ""), actor, MessageTypeDefOf.NeutralEvent, false);
                    this.TargetThingA.Destroy(0);
                    this.EndJobWith(JobCondition.Succeeded);
                }
            };
            yield break;
        }
    }
}
