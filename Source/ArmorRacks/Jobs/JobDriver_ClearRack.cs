using System.Collections.Generic;
using ArmorRacks.Things;
using ArmorRacks.Utils;
using RimWorld;
using Verse;
using Verse.AI;

namespace ArmorRacks.Jobs
{
    public class JobDriverClearRack : JobDriver_BaseRackJob
    {
        public bool ForbidAfter = false;
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            AddFailCondition(delegate
            {
                var rack = (ArmorRack) TargetThingA;
                if (!ArmorRackJobUtil.RackHasItems(rack))
                {
                    var text = "ArmorRacks_ClearRack_FloatMenuLabel_Empty".Translate(pawn.LabelShort);
                    Messages.Message(text, MessageTypeDefOf.RejectInput, false);
                    return true;
                }
                return false;
            });
            this.FailOnForbidden(TargetIndex.A);
            return base.TryMakePreToilReservations(errorOnFailed);
        }

        public virtual Toil DropToil
        {
            get
            {
                return new Toil()
                {
                    initAction = delegate
                    {
                        var armorRack = TargetThingA as ArmorRack;
                        armorRack.DropContents();
                    }
                };
            }
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return Toils_Reserve.Reserve(TargetIndex.A);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            yield return Toils_General.WaitWith(TargetIndex.A, 100, true);
            yield return DropToil;
        }
    }
}