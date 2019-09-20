using ArmorRacks.ThingComps;
using ArmorRacks.Things;
using RimWorld;
using Verse;
using Verse.AI;

namespace ArmorRacks.Jobs
{
    public abstract class JobDriver_WearRackBase: JobDriver
    {
        public int WaitTicks
        {
            get
            {
                var armorRack = (ArmorRack) TargetThingA;
                var totalEquipDelay = 0f;
                var rackApparel = armorRack.GetStoredApparel();
                var pawnApparel = pawn.apparel.WornApparel;
                var usedApparel = rackApparel.Count > pawnApparel.Count ? rackApparel : pawnApparel;
                foreach (var apparel in usedApparel)
                {
                    var equipDelay = apparel.GetStatValue(StatDefOf.EquipDelay);
                    totalEquipDelay += equipDelay;
                }
                float equipDelayFactor = armorRack.GetComp<ArmorRackComp>().Props.equipDelayFactor;
                var waitTicks = totalEquipDelay * equipDelayFactor * 60f;
                Log.Warning(waitTicks.ToString());
                return (int)waitTicks;
            }
            
        }
    }
}