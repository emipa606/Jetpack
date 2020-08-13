using System;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace JetPack
{
    // Token: 0x02000006 RID: 6
    public class Command_JetPack : Command
    {
        // Token: 0x0600000B RID: 11 RVA: 0x000022B8 File Offset: 0x000004B8
        internal static TargetingParameters ForJetPacksDestination()
        {
            TargetingParameters targetingParameters = new TargetingParameters();
            targetingParameters.canTargetLocations = true;
            targetingParameters.canTargetSelf = false;
            targetingParameters.canTargetPawns = false;
            targetingParameters.canTargetFires = false;
            targetingParameters.canTargetBuildings = false;
            targetingParameters.canTargetItems = false;
            targetingParameters.validator = ((TargetInfo x) => DropCellFinder.IsGoodDropSpot(x.Cell, x.Map, true, Command_JetPack.JPRoofPunch, true));
            return targetingParameters;
        }

        // Token: 0x0600000C RID: 12 RVA: 0x0000231C File Offset: 0x0000051C
        public override void ProcessInput(Event ev)
        {
            base.ProcessInput(ev);
            SoundStarter.PlayOneShotOnCamera(SoundDefOf.Tick_Tiny, null);
            Texture2D JPIcon = ContentFinder<Texture2D>.Get(Command_JetPack.JPIconPath, true);
            Find.Targeter.BeginTargeting(Command_JetPack.targParms, delegate (LocalTargetInfo target)
            {
                this.action(target.Cell);
            }, this.Pilot, null, JPIcon);
        }

        // Token: 0x0600000D RID: 13 RVA: 0x0000236C File Offset: 0x0000056C
        public override void GizmoUpdateOnMouseover()
        {
            if (Find.CurrentMap != null)
            {
                float MaxRadius = 0f;
                if (this.JPFRate > 0f)
                {
                    MaxRadius = (float)this.JPFuel / this.JPFRate;
                }
                if (MaxRadius > this.JPMaxJump)
                {
                    MaxRadius = this.JPMaxJump;
                }
                if (MaxRadius < this.JPMinJump)
                {
                    MaxRadius = this.JPMinJump;
                }
                if (MaxRadius > 0f)
                {
                    GenDraw.DrawRadiusRing(this.Pilot.Position, MaxRadius);
                }
                if (this.JPMinJump > 0f)
                {
                    GenDraw.DrawRadiusRing(this.Pilot.Position, this.JPMinJump);
                }
            }
        }

        // Token: 0x17000001 RID: 1
        // (get) Token: 0x0600000E RID: 14 RVA: 0x000023FE File Offset: 0x000005FE
        public override Color IconDrawColor
        {
            get
            {
                return base.IconDrawColor;
            }
        }

        // Token: 0x0600000F RID: 15 RVA: 0x00002406 File Offset: 0x00000606
        public override bool InheritInteractionsFrom(Gizmo other)
        {
            return false;
        }

        // Token: 0x04000001 RID: 1
        public Action<IntVec3> action;

        // Token: 0x04000002 RID: 2
        public Pawn Pilot;

        // Token: 0x04000003 RID: 3
        public int JPFuel;

        // Token: 0x04000004 RID: 4
        public float JPFRate;

        // Token: 0x04000005 RID: 5
        public float JPMaxJump;

        // Token: 0x04000006 RID: 6
        public float JPMinJump;

        // Token: 0x04000007 RID: 7
        public string JPSKFStr;

        // Token: 0x04000008 RID: 8
        public static bool JPRoofPunch = Settings.RoofPunch;

        // Token: 0x04000009 RID: 9
        internal static TargetingParameters targParms = Command_JetPack.ForJetPacksDestination();

        // Token: 0x0400000A RID: 10
        [NoTranslate]
        internal static string JPIconPath = "Things/Special/JetPackIcon";
    }
}
