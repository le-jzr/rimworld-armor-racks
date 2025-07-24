using ArmorRacks.DefOfs;
using ArmorRacks.Things;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace ArmorRacks.Commands
{
    public class ArmorRackUseCommand : Command
    {
        private readonly ArmorRackBase armorRack;
        private readonly Pawn pawn;

        public ArmorRackUseCommand(ArmorRackBase armorRack, Pawn pawn)
        {
            this.armorRack = armorRack;
            this.pawn = pawn;
            this.icon = ContentFinder<Texture2D>.Get(armorRack.def.graphicData.texPath + "_south", false);
            this.defaultIconColor = armorRack.Stuff.stuffProps.color;
        }

        public override string Label
        {
            get
            {
                if (this.armorRack.IsForbidden(this.pawn.Faction))
                {
                    return "ArmorRacks_TransferToRack_FloatMenuLabel".Translate();
                }
                else
                {
                    return "ArmorRacks_WearRack_FloatMenuLabel".Translate();
                }
            }
        }

        public override string Desc => Label;

        public override void ProcessInput(Event ev)
        {
            base.ProcessInput(ev);

            if (this.armorRack.IsForbidden(this.pawn.Faction))
                pawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(ArmorRacksJobDefOf.ArmorRacks_JobTransferToRack, armorRack), JobTag.Misc);
            else
                pawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(ArmorRacksJobDefOf.ArmorRacks_JobWearRack, armorRack), JobTag.Misc);
        }
    }
}
