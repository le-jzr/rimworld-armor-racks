using System.Collections.Generic;
using ArmorRacks.Things;
using RimWorld;
using Verse;
using Verse.AI;

namespace ArmorRacks.Jobs
{
    public class JobDriverTransferToRack : JobDriver_WearRackBase
    {
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
                    var storedRackApparel = new List<Apparel>(armorRack.GetStoredApparel());
                    var storedRackWeapon = armorRack.GetStoredWeapon();
                    var storedPawnApparel = new List<Apparel>(pawn.apparel.WornApparel);
                    var storedPawnWeapon = pawn.equipment.Primary;
                    Thing lastResultingThing;
                    
                    armorRack.InnerContainer.Clear();
                    foreach (var pawnApparel in storedPawnApparel)
                    {
                        if (armorRack.Accepts(pawnApparel))
                        {
                            pawn.apparel.Remove(pawnApparel);
                            armorRack.InnerContainer.TryAdd(pawnApparel);
                        }
                    }

                    var leftoverRackApparel = new List<Apparel>();
                    foreach (var rackApparel in storedRackApparel)
                    {
                        if (armorRack.Accepts(rackApparel))
                        {
                            armorRack.InnerContainer.TryAdd(rackApparel);
                        }
                        else
                        {
                            leftoverRackApparel.Add(rackApparel);
                        }
                    }

                    foreach (Apparel leftoverApparel in leftoverRackApparel)
                    {
                        if (ApparelUtility.HasPartsToWear(pawn, leftoverApparel.def) && pawn.apparel.CanWearWithoutDroppingAnything(leftoverApparel.def))
                        {
                            pawn.apparel.Wear(leftoverApparel);
                        }
                        else
                        {
                            GenDrop.TryDropSpawn(leftoverApparel, armorRack.Position, armorRack.Map,
                                ThingPlaceMode.Near, out lastResultingThing);
                        }
                    }
                    
                    int hasRackWeapon = storedRackWeapon == null ? 0x00 : 0x01;
                    int hasPawnWeapon = storedPawnWeapon == null ? 0x00 : 0x10;
                    switch (hasRackWeapon | hasPawnWeapon)
                    {
                        case 0x11:
                        {
                            if (armorRack.Accepts(storedPawnWeapon))
                            {
                                armorRack.InnerContainer.TryAdd(storedPawnWeapon);
                                pawn.equipment.MakeRoomFor((ThingWithComps)storedRackWeapon);
                                pawn.equipment.AddEquipment((ThingWithComps)storedRackWeapon);
                            }
                            else
                            {
                                armorRack.InnerContainer.TryAdd(storedRackWeapon);
                            }
                            break;
                        }
                        case 0x01:
                            armorRack.InnerContainer.TryAdd(storedRackWeapon);
                            break;
                        case 0x10:
                        {
                            if (armorRack.Accepts(storedPawnWeapon))
                            {
                                pawn.equipment.Remove(storedPawnWeapon);
                                armorRack.InnerContainer.TryAdd(storedPawnWeapon);
                            }
                            break;
                        }
                    }
                    ForbidUtility.SetForbidden(TargetThingA, false);
                }
            };
        }
    }
}