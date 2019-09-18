using System.Collections.Generic;
using ArmorRacks.Things;
using RimWorld;
using Verse;
using Verse.AI;

namespace ArmorRacks.Jobs
{
    public class JobDriverClearForbidRack : JobDriverClearRack
    {
        public override Toil GetDropToil()
        {
            return new Toil()
            {
                initAction = delegate
                {
                    var armorRack = TargetThingA as ArmorRack;
                    ForbidUtility.SetForbidden(armorRack, true);
                    armorRack.DropContents();
                }
            };
        }
    }
}