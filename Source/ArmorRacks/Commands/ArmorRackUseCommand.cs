using System.Collections.Generic;
using ArmorRacks.DefOfs;
using ArmorRacks.ThingComps;
using ArmorRacks.Things;
using UnityEngine;
using Verse;
using Verse.AI;

namespace ArmorRacks.Commands
{
    public class ArmorRackInteractCommand : Command
    {
        public ArmorRack ArmorRack;
        public Pawn Pawn;

        public ArmorRackInteractCommand(ArmorRack armorRack, Pawn pawn)
        {
            ArmorRack = armorRack;
            Pawn = pawn;
        }

        public override string Label
        {
            get
            {
                var selectedJobDef = Pawn.GetComp<ArmorRackUseCommandComp>().CurArmorRackJobDef;
                if (selectedJobDef == ArmorRacksJobDefOf.ArmorRacks_JobWearRack)
                {
                    return "ArmorRacks_WearRack_FloatMenuLabel".Translate();
                }

                return "ArmorRacks_SwapWithRack_FloatMenuLabel".Translate();
            }
        }

        public override string Desc => Label;

        public override IEnumerable<FloatMenuOption> RightClickFloatMenuOptions
        {
            get
            {
                // Equip from
                var self = this;
                yield return new FloatMenuOption("ArmorRacks_WearRack_FloatMenuLabel".Translate(),
                    delegate
                    {
                        Pawn.GetComp<ArmorRackUseCommandComp>().CurArmorRackJobDef = ArmorRacksJobDefOf.ArmorRacks_JobWearRack;
                    });
                
                // Swap with
                yield return new FloatMenuOption("ArmorRacks_SwapWithRack_FloatMenuLabel".Translate(),
                    delegate
                    {
                        Pawn.GetComp<ArmorRackUseCommandComp>().CurArmorRackJobDef = ArmorRacksJobDefOf.ArmorRacks_JobSwapWithRack;
                    });
            }
        }

        public override void ProcessInput(Event ev)
        {
            base.ProcessInput(ev);
            var target_info = new LocalTargetInfo(ArmorRack);
            var selectedJobDef = Pawn.GetComp<ArmorRackUseCommandComp>().CurArmorRackJobDef;
            var wearRackJob = new Job(selectedJobDef, target_info);
            Pawn.jobs.TryTakeOrderedJob(wearRackJob);
        }
    }
}