using System;
using System.Collections.Generic;
using ArmorRacks.Drawers;
using RimWorld;
using UnityEngine;
using Verse;

namespace ArmorRacks.Things
{
    public class ArmorRack : Building, IHaulDestination, IThingHolder
    {
        public StorageSettings Settings;
        public ThingOwner<Thing> InnerContainer;
        public ArmorRackContentsDrawer ContentsDrawer;
        public BodyDef BodyDef => BodyDefOf.Human;
        public BodyTypeDef BodyTypeDef => BodyTypeDefOf.Male;
        public bool StorageTabVisible => true;

        public ArmorRack()
        {
            InnerContainer = new ThingOwner<Thing>(this, false);
            ContentsDrawer = new ArmorRackContentsDrawer(this);
        }

        public StorageSettings GetStoreSettings()
        {
            return Settings;
        }

        public StorageSettings GetParentStoreSettings()
        {
            return def.building.fixedStorageSettings;
        }

        public override void PostMake()
        {
            base.PostMake();
            Settings = new StorageSettings(this);
            if (def.building.defaultStorageSettings == null)
                return;
            Settings.CopyFrom(def.building.defaultStorageSettings);
        }

        public bool Accepts(Thing t)
        {
            bool result = Settings.AllowedToAccept(t);
            if (result)
            {
                if (t.def.IsWeapon)
                {
                    result = CanStoreWeapon(t);
                }
                else if (t.def.IsApparel)
                {
                    result = CanStoreApparel((Apparel) t);
                }
            }
            return result;
        }

        public bool CanStoreWeapon(Thing weapon)
        {
            Thing storedWeapon = GetStoredWeapon();
            return storedWeapon == null;
        }

        public Thing GetStoredWeapon()
        {
            foreach (Thing storedThing in GetDirectlyHeldThings())
            {
                if (storedThing.def.IsWeapon)
                {
                    return storedThing;
                }
            }
            return null;
        } 

        public bool CanStoreApparel(Apparel apparel)
        {
            foreach (Apparel storedApparel in GetStoredApparel())
            {
                if (!ApparelUtility.CanWearTogether(storedApparel.def, apparel.def, BodyDef))
                {
                    return false;
                }
            }

            return true;
        }

        public IEnumerable<Apparel> GetStoredApparel()
        {
            foreach (Thing storedThing in GetDirectlyHeldThings())
            {
                if (storedThing.def.IsApparel)
                {
                    yield return (Apparel) storedThing;
                }
            }
        }

        public void GetChildHolders(List<IThingHolder> outChildren)
        {
            ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, GetDirectlyHeldThings());
        }

        public ThingOwner GetDirectlyHeldThings()
        {
            return InnerContainer;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Deep.Look(ref InnerContainer, "ArmorRackInnerContainer", this);
            Scribe_Deep.Look(ref Settings, "ArmorRackSettings", this);
        }

        public override void Draw()
        {
            this.DrawAt(DrawPos);
        }

        public override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            base.DrawAt(drawLoc, flip);
            ContentsDrawer.DrawAt(drawLoc);
        }

        public void DropContents()
        {
            IntVec3 dropPos = new IntVec3(DrawPos);
            InnerContainer.TryDropAll(dropPos, Map, ThingPlaceMode.Near);
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            DropContents();
            base.Destroy(mode);
        }
    }
}