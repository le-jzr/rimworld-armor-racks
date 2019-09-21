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
            return base.TryMakePreToilReservations(errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            foreach (var toil in base.MakeNewToils())
            {
                yield return toil;
            }
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