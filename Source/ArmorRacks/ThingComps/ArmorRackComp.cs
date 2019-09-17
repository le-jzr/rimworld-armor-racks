using System;
using System.Collections.Generic;
using System.Linq;
using ArmorRacks.Commands;
using ArmorRacks.DefOfs;
using ArmorRacks.Things;
using ArmorRacks.Utils;
using RimWorld;
using Verse;
using Verse.AI;

namespace ArmorRacks.ThingComps
{
    public class ArmorRackCompProperties : CompProperties
    {
        public bool myExampleBool;

        public ArmorRackCompProperties()
        {
            this.compClass = typeof(ArmorRackComp);
        }

        public ArmorRackCompProperties(Type compClass) : base(compClass)
        {
            this.compClass = compClass;
        }
    }

    public class ArmorRackComp : ThingComp
    {
        public ArmorRackCompProperties Props => (ArmorRackCompProperties) this.props;

        public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
        {
            ArmorRack armorRack = this.parent as ArmorRack;
            if (ForbidUtility.IsForbidden(armorRack, selPawn))
            {
                yield break;
            }
            
            if (!selPawn.CanReach(armorRack, PathEndMode.Touch, Danger.Deadly, false, TraverseMode.ByPawn))
            {
                FloatMenuOption failer = new FloatMenuOption("CannotUseNoPath".Translate(), null, MenuOptionPriority.Default, null, null, 0f, null, null);
                yield return failer;
                yield break;
            }

            var nonViolentOptionYielded = false;
            if (ArmorRackJobUtil.PawnCanEquipWeaponSet(armorRack, selPawn))
            {
                // Swap with
                var swapWithOption = new FloatMenuOption("ArmorRacks_SwapRackFloatMenuLabel".Translate(), delegate
                {
                    var target_info = new LocalTargetInfo(armorRack);
                    var wearRackJob = new Job(ArmorRacksJobDefOf.ArmorRacks_JobSwapWithRack, target_info);
                    selPawn.jobs.TryTakeOrderedJob(wearRackJob);
                });
                yield return FloatMenuUtility.DecoratePrioritizedTask(swapWithOption, selPawn, armorRack, "ReservedBy");
            }
            else
            {
                yield return new FloatMenuOption("ArmorRacks_CannotEquipNonviolent".Translate(), null);
                nonViolentOptionYielded = true;
            }

            if (ArmorRackJobUtil.RackHasItems(armorRack))
            {
                if (ArmorRackJobUtil.PawnCanEquipWeaponSet(armorRack, selPawn))
                {
                    // Equip from
                    var equipFromOption = new FloatMenuOption("ArmorRacks_WearRackFloatMenuLabel".Translate(), delegate
                    {
                        var target_info = new LocalTargetInfo(armorRack);
                        var wearRackJob = new Job(ArmorRacksJobDefOf.ArmorRacks_JobWearRack, target_info);
                        selPawn.jobs.TryTakeOrderedJob(wearRackJob);
                    });
                    yield return FloatMenuUtility.DecoratePrioritizedTask(equipFromOption, selPawn, armorRack, "ReservedBy");
                }
                else if (!nonViolentOptionYielded)
                {
                    yield return new FloatMenuOption("ArmorRacks_CannotEquipNonviolent".Translate(), null);
                }
                
                // Clear out
                var clearOutOption = new FloatMenuOption("ArmorRacks_ClearRackFloatMenuLabel".Translate(), delegate
                {
                    var target_info = new LocalTargetInfo(armorRack);
                    var clearRackJob = new Job(ArmorRacksJobDefOf.ArmorRacks_JobClearRack, target_info);
                    selPawn.jobs.TryTakeOrderedJob(clearRackJob);
                });
                yield return FloatMenuUtility.DecoratePrioritizedTask(clearOutOption, selPawn, armorRack, "ReservedBy");
            
                // Clear out and forbid
                var clearOutForbidOption = new FloatMenuOption("ArmorRacks_ClearForbidRackFloatMenuLabel".Translate(), delegate
                {
                    var target_info = new LocalTargetInfo(armorRack);
                    var clearRackJob = new Job(ArmorRacksJobDefOf.ArmorRacks_JobClearForbidRack, target_info);
                    selPawn.jobs.TryTakeOrderedJob(clearRackJob);
                });
                yield return FloatMenuUtility.DecoratePrioritizedTask(clearOutForbidOption, selPawn, armorRack, "ReservedBy");
            }
            else
            {
                yield return new FloatMenuOption("ArmorRacks_WearRackFloatMenuLabel_Empty".Translate(), null);
                yield return new FloatMenuOption("ArmorRacks_ClearRackFloatMenuLabel_Empty".Translate(), null);
            }
            
        }
    }
    

    public class ArmorRackUseCommandComp : ThingComp
    {
        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (parent is Pawn pawn)
            {
                var racks = pawn.Map.listerBuildings.AllBuildingsColonistOfClass<ArmorRack>();
                foreach (var rack in racks)
                {
                    if (rack.AssignedAnything(pawn))
                    {
                        yield return new ArmorRackWearCommand(rack, pawn);    
                        yield return new ArmorRackSwapCommand(rack, pawn);   
                    }
                }
            }
        }
    }

}