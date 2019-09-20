using System.Collections.Generic;
using System.Linq;
using ArmorRacks.ThingComps;
using ArmorRacks.Things;
using ArmorRacks.Utils;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace ArmorRacks.Jobs
{
    public class JobDriverWearRack : JobDriver_WearRackBase
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            AddFailCondition(delegate
            {
                var rack = (ArmorRack) TargetThingA;
                if (!ArmorRackJobUtil.RackHasItems(rack))
                {
                    var text = "ArmorRacks_WearRack_JobFailMessage_Empty".Translate(pawn.LabelShort);
                    Messages.Message(text, MessageTypeDefOf.RejectInput, false);
                    return true;
                }
                if (!ArmorRackJobUtil.PawnCanEquipWeaponSet(rack, pawn))
                {
                    var text = "ArmorRacks_WearRack_JobFailMessage_NonViolent".Translate(pawn.LabelShort);
                    Messages.Message(text, MessageTypeDefOf.RejectInput, false);
                    return true;
                }
                return false;
            });
            return pawn.Reserve(TargetThingA, job, errorOnFailed: errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDestroyedNullOrForbidden(TargetIndex.A);
            yield return Toils_Reserve.Reserve(TargetIndex.A);
            var destination = TargetThingA.def.hasInteractionCell ? PathEndMode.InteractionCell : PathEndMode.Touch;
            yield return Toils_Goto.GotoThing(TargetIndex.A, destination);
            yield return Toils_General.WaitWith(TargetIndex.A, WaitTicks, true);
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