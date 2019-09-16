using System.Collections.Generic;
using ArmorRacks.Things;
using RimWorld;
using Verse;
using Verse.AI;

namespace ArmorRacks.Jobs
{
    public class JobDriverClearForbidRack : JobDriverClearRack
    {
        protected override IEnumerable<Toil> MakeNewToils()
        {
            foreach (var toil in base.MakeNewToils())
            {
                yield return toil;
            }
            yield return Toils_Misc.SetForbidden(TargetIndex.A, true);
        }
    }
}