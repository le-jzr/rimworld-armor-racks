using ArmorRacks.DefOfs;
using ArmorRacks.Things;
using UnityEngine;
using Verse;
using Verse.AI;

namespace ArmorRacks.Commands
{
    public class ArmorRackWearCommand : Command
    {
        public ArmorRack ArmorRack;
        public Pawn Pawn;

        public ArmorRackWearCommand(ArmorRack armorRack, Pawn pawn)
        {
            defaultLabel = "ArmorRacks_WearRackFloatMenuLabel".Translate();
            defaultDesc = "ArmorRacks_WearRackFloatMenuLabel".Translate();
            ArmorRack = armorRack;
            Pawn = pawn;
        }

        public override void ProcessInput(Event ev)
        {
            base.ProcessInput(ev);
            var target_info = new LocalTargetInfo(ArmorRack);
            var wearRackJob = new Job(ArmorRacksJobDefOf.ArmorRacks_JobWearRack, target_info);
            Pawn.jobs.TryTakeOrderedJob(wearRackJob);
        }
    }
    
    public class ArmorRackSwapCommand : Command
    {
        public ArmorRack ArmorRack;
        public Pawn Pawn;

        public ArmorRackSwapCommand(ArmorRack armorRack, Pawn pawn)
        {
            defaultLabel = "ArmorRacks_SwapRackFloatMenuLabel".Translate();
            defaultDesc = "ArmorRacks_SwapRackFloatMenuLabel".Translate();
            ArmorRack = armorRack;
            Pawn = pawn;
        }

        public override void ProcessInput(Event ev)
        {
            base.ProcessInput(ev);
            var target_info = new LocalTargetInfo(ArmorRack);
            var swapRackJob = new Job(ArmorRacksJobDefOf.ArmorRacks_JobSwapWithRack, target_info);
            Pawn.jobs.TryTakeOrderedJob(swapRackJob);
        }
    }
}