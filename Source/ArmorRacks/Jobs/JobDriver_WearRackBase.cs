using ArmorRacks.ThingComps;
using ArmorRacks.Things;
using RimWorld;

namespace ArmorRacks.Jobs
{
    public abstract class JobDriver_WearRackBase : JobDriver_BaseRackJob
    {
        public override int WaitTicks
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

                var armorRackProps = armorRack.GetComp<ArmorRackComp>().Props;
                var powerOn = armorRack.GetComp<CompPowerTrader>().PowerOn;
                float equipDelayFactor = powerOn ? armorRackProps.equipDelayFactorPowered : armorRackProps.equipDelayFactor;
                var waitTicks = totalEquipDelay * equipDelayFactor * 60f;
                return (int) waitTicks;
            }
        }
    }
}