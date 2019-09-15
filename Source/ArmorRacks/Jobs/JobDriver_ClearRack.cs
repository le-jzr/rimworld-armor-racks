using System.Collections.Generic;
using ArmorRacks.Things;
using Verse;
using Verse.AI;

namespace ArmorRacks.Jobs
{
    public class JobDriverClearRack : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            if (!pawn.CanReserve(TargetThingA))
            {
                return false;
            }

            return pawn.Reserve(TargetThingA, job, errorOnFailed: errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDestroyedNullOrForbidden(TargetIndex.A);
            yield return Toils_Reserve.Reserve(TargetIndex.A);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            yield return Toils_General.Wait(100, TargetIndex.A);
            yield return new Toil()
            {
                initAction = delegate
                {
                    var armorRack = TargetThingA as ArmorRack;
                    armorRack.DropContents();
                }
            };
        }
    }
}