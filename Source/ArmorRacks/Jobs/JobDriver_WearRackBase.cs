using System;
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
                var pawnTotalEquipDelay = 0f;
                var rackTotalEquipDelay = 0f;
                var rackApparel = armorRack.GetStoredApparel();
                var pawnApparel = pawn.apparel.WornApparel;
                
                foreach (var apparel in rackApparel)
                {
                    var equipDelay = apparel.GetStatValue(StatDefOf.EquipDelay);
                    rackTotalEquipDelay += equipDelay;
                }
                foreach (var apparel in pawnApparel)
                {
                    var equipDelay = apparel.GetStatValue(StatDefOf.EquipDelay);
                    pawnTotalEquipDelay += equipDelay;
                }
                var totalEquipDelay = Math.Max(rackTotalEquipDelay, pawnTotalEquipDelay);

                var armorRackProps = armorRack.GetComp<ArmorRackComp>().Props;
                var powerComp = armorRack.GetComp<CompPowerTrader>();
                var powerOn = powerComp != null && powerComp.PowerOn;
                float equipDelayFactor = powerOn ? armorRackProps.equipDelayFactorPowered : armorRackProps.equipDelayFactor;
                var waitTicks = totalEquipDelay * equipDelayFactor * 60f;
                return (int) waitTicks;
            }
        }
    }
}