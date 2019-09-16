using System;
using System.Collections.Generic;
using ArmorRacks.DefOfs;
using ArmorRacks.Jobs;
using ArmorRacks.Things;
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
        public ArmorRackCompProperties Props => (ArmorRackCompProperties)this.props;
        public bool ExampleBool => Props.myExampleBool;

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
            
            if (armorRack.InnerContainer.Count != 0)
            {
                // Equip from
                if (selPawn.story.WorkTagIsDisabled(WorkTags.Violent) && armorRack.GetStoredWeapon() != null)
                {
                    yield return new FloatMenuOption("Cannot equip from rack (rack has weapon but pawn is non-violent)", null);
                }
                else
                {
                    var equipFromOption = new FloatMenuOption("Equip from armor rack", delegate
                    {
                        var target_info = new LocalTargetInfo(armorRack);
                        var wearRackJob = new Job(ArmorRacksJobDefOf.ArmorRacks_JobWearRack, target_info);
                        selPawn.jobs.TryTakeOrderedJob(wearRackJob);
                    });
                    yield return FloatMenuUtility.DecoratePrioritizedTask(equipFromOption, selPawn, armorRack, "ReservedBy");
                    
                    var swapWithOption = new FloatMenuOption("Swap with armor rack", delegate
                    {
                        var target_info = new LocalTargetInfo(armorRack);
                        var wearRackJob = new Job(ArmorRacksJobDefOf.ArmorRacks_JobSwapWithRack, target_info);
                        selPawn.jobs.TryTakeOrderedJob(wearRackJob);
                    });
                    yield return FloatMenuUtility.DecoratePrioritizedTask(swapWithOption, selPawn, armorRack, "ReservedBy");
                }
                
                // Clear out
                var clearOutOption = new FloatMenuOption("Clear out armor rack", delegate
                {
                    var target_info = new LocalTargetInfo(armorRack);
                    var clearRackJob = new Job(ArmorRacksJobDefOf.ArmorRacks_JobClearRack, target_info);
                    selPawn.jobs.TryTakeOrderedJob(clearRackJob);
                });
                yield return FloatMenuUtility.DecoratePrioritizedTask(clearOutOption, selPawn, armorRack, "ReservedBy");
            
                // Clear out and forbid
                var clearOutForbidOption = new FloatMenuOption("Clear out and forbid armor rack", delegate
                {
                    var target_info = new LocalTargetInfo(armorRack);
                    var clearRackJob = new Job(ArmorRacksJobDefOf.ArmorRacks_JobClearForbidRack, target_info);
                    selPawn.jobs.TryTakeOrderedJob(clearRackJob);
                });
                yield return FloatMenuUtility.DecoratePrioritizedTask(clearOutForbidOption, selPawn, armorRack, "ReservedBy");
            }
            else
            {
                yield return new FloatMenuOption("Cannot equip from rack (empty)", null);
                yield return new FloatMenuOption("Cannot clear out rack (empty)", null);
            }
        }
    }
}