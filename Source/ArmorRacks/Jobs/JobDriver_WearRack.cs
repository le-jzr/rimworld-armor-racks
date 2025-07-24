using System.Collections.Generic;
using System.Linq;
using ArmorRacks.Things;
using RimWorld;
using Verse;
using Verse.AI;

namespace ArmorRacks.Jobs
{
    public class JobDriver_WearRack : JobDriver_UseOutfitStand
    {
        protected ArmorRackBase Rack => (ArmorRackBase)job.targetA.Thing;

        protected List<Apparel> rackApparel = new List<Apparel>();

        protected float durationMultiplier = 1.0f;

        protected override IEnumerable<Toil> MakeNewToils()
        {
            foreach (var toil in base.MakeNewToils())
            {
                if (toil.defaultDuration > 0)
                    toil.defaultDuration = (int)(((float)toil.defaultDuration) * durationMultiplier);

                yield return toil;
            }

            Toil fin = ToilMaker.MakeToil("MakeNewToils");
            fin.AddFinishAction(FinalizeJob);
            fin.defaultCompleteMode = ToilCompleteMode.Instant;
            yield return fin;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref rackApparel, "ArmorRackJob.rackApparel", LookMode.Reference);
            Scribe_Values.Look(ref durationMultiplier, "ArmorRackJob.durationMultiplier");
        }

        public override void Notify_Starting()
        {
            base.Notify_Starting();
            this.rackApparel.Clear();
            this.rackApparel.AddRange(Rack.HeldItems.AsEnumerable().OfType<Apparel>());
        }

        protected virtual void FinalizeJob()
        {
            bool setForced = LoadedModManager.GetMod<ArmorRacksMod>().GetSettings<ArmorRacksModSettings>().EquipSetForced;

            foreach (var item in this.rackApparel)
                pawn.outfits.forcedHandler.SetForced(item, forced: setForced);

            this.rackApparel.Clear();

            ForbidUtility.SetForbidden(TargetThingA, true);
        }
    }
}
