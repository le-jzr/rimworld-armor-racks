using System.Collections.Generic;
using System.Linq;
using ArmorRacks.Things;
using RimWorld;
using Verse;
using Verse.AI;

namespace ArmorRacks.Jobs
{
    public class JobDriverWearRack : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(TargetThingA, job, errorOnFailed: errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDestroyedNullOrForbidden(TargetIndex.A);
            yield return Toils_Reserve.Reserve(TargetIndex.A);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            yield return Toils_General.WaitWith(TargetIndex.A, 100, true);
            yield return new Toil()
            {
                initAction = delegate
                {
                    var armorRack = TargetThingA as ArmorRack;
                    foreach (Apparel rackApparel in armorRack.GetStoredApparel())
                    {
                        armorRack.InnerContainer.Remove(rackApparel);
                        pawn.apparel.Wear(rackApparel, true);
                    }

                    ThingWithComps rackWeapon = (ThingWithComps)armorRack.GetStoredWeapon();
                    if (rackWeapon != null)
                    {
                        armorRack.InnerContainer.Remove(rackWeapon);
                        pawn.equipment.MakeRoomFor(rackWeapon);
                        pawn.equipment.AddEquipment(rackWeapon);   
                    }
                }
            };
        }
    }
}