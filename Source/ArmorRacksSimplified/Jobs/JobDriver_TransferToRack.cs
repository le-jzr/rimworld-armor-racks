using System.Collections.Generic;
using System.Linq;
using ArmorRacksSimplified.Things;
using RimWorld;
using Verse;
using Verse.AI;

namespace ArmorRacksSimplified.Jobs
{
    public class JobDriver_TransferToRack : JobDriver
    {
        private int duration;

        private HashSet<Apparel> wornApparelToTransferToStand = new HashSet<Apparel>();

        private HashSet<Apparel> standApparelToTransferToPawn = new HashSet<Apparel>();

        private ArmorRackBase OutfitStand => (ArmorRackBase)job.targetA.Thing;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref duration, "duration", 0);
            Scribe_Collections.Look(ref wornApparelToTransferToStand, "wornApparelToTransferToStand", LookMode.Reference);
            Scribe_Collections.Look(ref standApparelToTransferToPawn, "standApparelToTransferToPawn", LookMode.Reference);
        }

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(job.targetA, job, 1, -1, null, errorOnFailed);
        }

        public override void Notify_Starting()
        {
            bool setFilter = false;//OutfitStand.AutoSetStorageOnTransfer;

            base.Notify_Starting();
            standApparelToTransferToPawn.Clear();
            wornApparelToTransferToStand.Clear();
            var heldItems = OutfitStand.HeldItems;
            duration = 0;

            foreach (var item in pawn.apparel.WornApparel)
            {
                if (!setFilter && !OutfitStand.StoreSettings.AllowedToAccept(item))
                    continue;

                wornApparelToTransferToStand.Add(item);

                foreach (var item2 in heldItems.OfType<Apparel>())
                {
                    if (!ApparelUtility.CanWearTogether(item.def, item2.def, pawn.RaceProps.body))
                        standApparelToTransferToPawn.Add(item2);
                }
            }

            foreach (var item in standApparelToTransferToPawn)
                duration += (int)(item.GetStatValue(StatDefOf.EquipDelay) * 60f);

            foreach (var item in wornApparelToTransferToStand)
                duration += (int)(item.GetStatValue(StatDefOf.EquipDelay) * 60f);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnBurningImmobile(TargetIndex.A);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell).FailOnDespawnedNullOrForbidden(TargetIndex.A);
            Toil toil = ToilMaker.MakeToil("MakeNewToils");
            toil.WithProgressBarToilDelay(TargetIndex.A);
            toil.defaultCompleteMode = ToilCompleteMode.Delay;
            toil.defaultDuration = duration;
            yield return toil;
            Toil toil2 = ToilMaker.MakeToil("MakeNewToils");
            toil2.AddFinishAction(DoTransfer);
            toil2.defaultCompleteMode = ToilCompleteMode.Instant;
            yield return toil2;
        }

        private void DoTransfer()
        {
            bool setForced = LoadedModManager.GetMod<ArmorRacksSimplifiedMod>().GetSettings<ArmorRacksSimplifiedModSettings>().TransferSetForced;
            bool setFilter = false; //OutfitStand.AutoSetStorageOnTransfer;
            //OutfitStand.AutoSetStorageOnTransfer = false;

            if (setFilter)
                OutfitStand.StoreSettings.filter.SetDisallowAll();

            foreach (Apparel item in standApparelToTransferToPawn)
            {
                if (!OutfitStand.RemoveApparel(item))
                    Log.Warning("Could not remove apparel from Armor Rack.");
            }

            foreach (Apparel item in wornApparelToTransferToStand)
            {
                pawn.apparel.Remove(item);

                if (setFilter)
                    OutfitStand.StoreSettings.filter.SetAllow(item.def, true);

                if (!OutfitStand.AddApparel(item))
                {
                    Log.Warning("Could not add apparel to Armor Rack.");
                    GenDrop.TryDropSpawn(item, OutfitStand.Position, OutfitStand.Map, ThingPlaceMode.Near, out Thing _);
                }
            }

            foreach (Apparel item in standApparelToTransferToPawn)
            {
                if (ArmorRackBase.WearableByPawn(item, pawn, false) && pawn.apparel.CanWearWithoutDroppingAnything(item.def))
                {
                    pawn.apparel.Wear(item);
                    pawn.outfits.forcedHandler.SetForced(item, forced: setForced);
                }
                else
                {
                    GenDrop.TryDropSpawn(item, OutfitStand.Position, OutfitStand.Map, ThingPlaceMode.Near, out Thing _);
                }
            }

            var pawnWeapon = pawn.equipment.Primary;
            if (pawnWeapon != null && OutfitStand.StoreSettings.AllowedToAccept(pawnWeapon))
            {
                pawn.equipment.Remove(pawnWeapon);

                var heldWeapon = OutfitStand.HeldWeapon;
                if (heldWeapon == null)
                {
                    OutfitStand.AddOrDropWeapon(pawnWeapon);
                }
                else
                {
                    OutfitStand.RemoveHeldWeapon(heldWeapon);
                    OutfitStand.AddOrDropWeapon(pawnWeapon);
                    if (ArmorRackBase.WearableByPawn(heldWeapon, pawn, false))
                    {
                        pawn.equipment.AddEquipment(heldWeapon);
                    }
                    else
                    {
                        GenDrop.TryDropSpawn(heldWeapon, OutfitStand.Position, OutfitStand.Map, ThingPlaceMode.Near, out Thing _);
                    }
                }
            }

            ForbidUtility.SetForbidden(TargetThingA, false);

            this.standApparelToTransferToPawn.Clear();
            this.wornApparelToTransferToStand.Clear();
        }
    }
}
