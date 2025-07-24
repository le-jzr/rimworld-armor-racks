using System.Collections.Generic;
using System.Linq;
using ArmorRacksSimplified.Commands;
using ArmorRacksSimplified.DefOfs;
using ArmorRacksSimplified.ThingComps;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace ArmorRacksSimplified.Things
{
    public abstract class ArmorRackBase : Building_OutfitStand, IApparelSource, IHaulDestination
    {
        bool IApparelSource.ApparelSourceEnabled => false;
        protected bool forceTransparent = false;

        public static bool AutoSetStorageOnTransfer = false;

        protected abstract Graphic_Multi RackGraphic { get; }
        protected virtual Graphic_Multi RackHeadGraphic { get { return null; } }

        public override Color DrawColor
        {
            get
            {
                if (this.forceTransparent)
                    return new Color(0, 0, 0, 0);
                else
                    return base.DrawColor;
            }
        }

        public override Color DrawColorTwo
        {
            get
            {
                if (this.forceTransparent)
                    return new Color(0, 0, 0, 0);
                else
                    return base.DrawColor;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref AutoSetStorageOnTransfer, "ArmorRacks.AutoSetStorageOnTransfer");
        }

        protected override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            // Draw the rack itself.
            var rot = flip ? base.Rotation.Opposite : base.Rotation;
            var rackGraphic = RackGraphic;
            rackGraphic.GetColoredVersion(rackGraphic.Shader, DrawColor, DrawColorTwo).Draw(drawLoc, rot, this);

            // Draw the head "pin" above non-hat apparel, unless the rack is facing away from us.
            if (rot != Rot4.North)
            {
                var v = drawLoc.WithY(AltitudeLayer.ItemImportant.AltitudeFor() - 0.05f);
                var rackHeadGraphic = RackHeadGraphic;
                rackHeadGraphic?.GetColoredVersion(rackHeadGraphic.Shader, DrawColor, DrawColorTwo).Draw(v, rot, this);
            }

            // Trick the base class to draw the apparel without the hardcoded outfit stand graphic.
            this.forceTransparent = true;
            base.DrawAt(drawLoc, flip);
            this.forceTransparent = false;
        }

        public void PokeItems()
        {
            // "Notify" the base class that items changed so it recaches graphics.
            var item = (this as IThingHolder).GetDirectlyHeldThings().FirstOrFallback();
            if (item != null)
                base.Notify_ItemAdded(item);
        }

        private BodyTypeDef cachedBodyTypeDef;
        private PawnKindDef cachedPawnKindDef;

        protected override BodyTypeDef BodyTypeDefForRendering
        {
            get
            {
                if (cachedBodyTypeDef == null)
                    cachedBodyTypeDef = this.GetAssignedPawn()?.story?.bodyType ?? BodyTypeDefOf.Male;

                return cachedBodyTypeDef;
            }
        }

        protected PawnKindDef PawnKindDef
        {
            get
            {
                if (cachedPawnKindDef == null)
                    cachedPawnKindDef = this.GetAssignedPawn()?.kindDef ?? PawnKindDefOf.Colonist;

                return cachedPawnKindDef;
            }
        }

        public static bool WearableByPawn(Thing thing, Pawn pawn, bool shared)
        {
            if (thing is Apparel apparel)
            {
                if (!apparel.PawnCanWear(pawn))
                    return false;

                if (!shared && !ApparelUtility.HasPartsToWear(pawn, apparel.def))
                    return false;

                if (CompBiocodable.IsBiocoded(apparel))
                    return CompBiocodable.IsBiocodedFor(apparel, pawn);

                return true;
            }
            else
            {
                if (!shared && thing.def.IsWeapon && pawn.WorkTagIsDisabled(WorkTags.Violent))
                    return false;

                if (!shared && thing.def.IsRangedWeapon && pawn.WorkTagIsDisabled(WorkTags.Shooting))
                    return false;

                if (!shared && !pawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
                    return false;

                if (pawn.IsQuestLodger() && !EquipmentUtility.QuestLodgerCanEquip(thing, pawn))
                    return false;

                return EquipmentUtility.CanEquip(thing, pawn);
            }
        }
        bool WearableByAllOwners(Thing thing)
        {
            var comp = this.GetComp<CompAssignableToPawn_ArmorRacks>();
            int owners = comp.AssignedPawnsForReading.Count();

            if (owners == 0 && CompBiocodable.IsBiocoded(thing))
                return false;

            foreach (var pawn in comp.AssignedPawnsForReading)
            {
                if (!WearableByPawn(thing, pawn, owners > 1))
                    return false;
            }
            return true;
        }

        public void AddOrDropWeapon(Thing weapon)
        {
            if (this.TryAddHeldWeapon(weapon))
                return;

            GenDrop.TryDropSpawn(weapon, this.Position, this.Map, ThingPlaceMode.Near, out Thing _);
        }

        public void AddOrDropApparel(Apparel apparel)
        {
            if (this.AddApparel(apparel))
                return;

            GenDrop.TryDropSpawn(apparel, this.Position, this.Map, ThingPlaceMode.Near, out Thing _);
        }

        public void OwnerChanged()
        {
            cachedBodyTypeDef = null;
            cachedPawnKindDef = null;
            this.PokeItems();
        }

        bool IHaulDestination.Accepts(Thing t)
        {
            if (!GetStoreSettings().AllowedToAccept(t))
                return false;

            if (!WearableByAllOwners(t))
                return false;

            if (HeldItems.Contains(t))
                return true;

            if (t is Apparel)
                return HasRoomForApparelOfDef(t.def);

            return t.def.IsWeapon && HeldWeapon == null;
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (var g in base.GetGizmos())
                yield return g;

            yield return new ArmorRackAutoStorageCommand(this);
        }

        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn selPawn)
        {
            if (!selPawn.IsColonistPlayerControlled)
                yield break;

            if (!selPawn.CanReach(this, PathEndMode.InteractionCell, Danger.Deadly))
            {
                yield return new FloatMenuOption("CannotSwapOutfit".Translate().CapitalizeFirst() + ": " + "NoPath".Translate(), null);
                yield break;
            }

            if (!base.HeldItems.Any())
            {
                yield return new FloatMenuOption("CannotSwapOutfit".Translate().CapitalizeFirst() + ": " + "OutfitStandEmpty".Translate(), null);
                yield break;
            }

            // Equip from
            var self = this;
            var option = new FloatMenuOption("ArmorRacks_WearRack_FloatMenuLabel".Translate(),
                delegate
                {
                    selPawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(ArmorRacksJobDefOf.ArmorRacksSimplified_JobWearRack, this), JobTag.Misc);
                });

            yield return FloatMenuUtility.DecoratePrioritizedTask(option, selPawn, this);

            // Transfer to
            option = new FloatMenuOption("ArmorRacks_TransferToRack_FloatMenuLabel".Translate(),
                delegate
                {
                    selPawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(ArmorRacksJobDefOf.ArmorRacksSimplified_JobTransferToRack, this), JobTag.Misc);
                });

            yield return FloatMenuUtility.DecoratePrioritizedTask(option, selPawn, this);

            foreach (FloatMenuOption floatMenuOption in HaulSourceUtility.GetFloatMenuOptions(this, selPawn))
                yield return floatMenuOption;
        }
    }
}
