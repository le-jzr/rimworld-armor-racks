using System.Collections.Generic;
using ArmorRacksSimplified.Commands;
using ArmorRacksSimplified.Things;
using Verse;

namespace ArmorRacksSimplified.ThingComps
{
    public class ArmorRackUseCommandComp : ThingComp
    {
        public ArmorRackBase assignedRack;

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_References.Look(ref assignedRack, "assignedRack");
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            var pawn = (Pawn)parent;

            if (this.assignedRack == null)
                yield break;

            var command = new ArmorRackUseCommand(this.assignedRack, pawn);

            if (pawn.health.Downed)
                command.Disable("IsIncapped".Translate(pawn.LabelShort, pawn));

            yield return command;
        }
    }
}
