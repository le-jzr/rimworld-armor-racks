using System.Collections.Generic;
using ArmorRacks.Commands;
using ArmorRacks.DefOfs;
using ArmorRacks.Things;
using Verse;

namespace ArmorRacks.ThingComps
{
    public class ArmorRackUseCommandComp : ThingComp
    {
        public JobDef CurArmorRackJobDef = ArmorRacksJobDefOf.ArmorRacks_JobWearRack;
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Defs.Look(ref CurArmorRackJobDef, "CurArmorRackJobDef");
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (parent is Pawn pawn)
            {
                var racks = pawn.Map.listerBuildings.AllBuildingsColonistOfClass<ArmorRack>();
                foreach (var rack in racks)
                {
                    if (rack.AssignedPawn == pawn)
                    {
                        yield return new ArmorRackUseCommand(rack, pawn);
                    }
                }
            }
        }
    }
}