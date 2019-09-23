using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArmorRacks.Drawers;
using ArmorRacks.ThingComps;
using RimWorld;
using UnityEngine;
using Verse;

namespace ArmorRacks.Things
{
    public class ArmorRackInnerContainer : ThingOwner<Thing>
    {
        public ArmorRackInnerContainer()
        {
        }

        public ArmorRackInnerContainer(IThingHolder owner)
            : base(owner)
        {
        }

        public ArmorRackInnerContainer(IThingHolder owner, bool oneStackOnly, LookMode contentsLookMode = LookMode.Deep)
            : base(owner, oneStackOnly, contentsLookMode)
        {
        }

        public override bool TryAdd(Thing item, bool canMergeWithExistingStacks = true)
        {
            var result = base.TryAdd(item, canMergeWithExistingStacks);
            var armorRack = owner as ArmorRack; 
            armorRack.ContentsChanged(item);
            return result;
        }

        public override bool Remove(Thing item)
        {
            var result = base.Remove(item);
            var armorRack = owner as ArmorRack;
            armorRack.ContentsChanged(item);
            return result;
        }
    }
    
    public class ArmorRack : Building, IAssignableBuilding, IHaulDestination, IThingHolder
    {
        public StorageSettings Settings;
        public ArmorRackInnerContainer InnerContainer;
        public ArmorRackContentsDrawer ContentsDrawer;
        public BodyDef BodyDef => BodyDefOf.Human;
        public BodyTypeDef BodyTypeDef => BodyTypeDefOf.Male;
        public bool StorageTabVisible => true;
        public Pawn AssignedPawn;

        public ArmorRack()
        {
            InnerContainer = new ArmorRackInnerContainer(this, false);
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
            var innerList = InnerContainer.InnerListForReading;
            foreach (Thing storedThing in innerList)
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

        public List<Apparel> GetStoredApparel()
        {
            var innerList = InnerContainer.InnerListForReading;
            var result = new List<Apparel>();
            foreach (Thing storedThing in innerList)
            {
                if (storedThing.def.IsApparel)
                {
                    result.Add((Apparel) storedThing);
                }
            }
            return result;
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
            Scribe_References.Look(ref AssignedPawn, "AssignedPawn");
        }
        

        public override void Draw()
        {
            DrawAt(DrawPos);
            Comps_PostDraw();
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

        public virtual void ContentsChanged(Thing thing)
        {
            ContentsDrawer.IsApparelResolved = false;
        }
        
        public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
        {
            UnassignPawn();
            base.DeSpawn(mode);
        }

        public IEnumerable<Pawn> AssigningCandidates
        {
            get
            {
                if (!this.Spawned)
                    return Enumerable.Empty<Pawn>();
                return this.Map.mapPawns.FreeColonists;
            }
        }

        public int MaxAssignedPawnsCount => 1;

        public IEnumerable<Pawn> AssignedPawns
        {
            get
            {
                if (AssignedPawn != null)
                {
                    yield return AssignedPawn;
                }
            }
        }

        public void TryAssignPawn(Pawn pawn)
        {
            AssignedPawn = pawn;
        }

        public void TryUnassignPawn(Pawn pawn)
        {
            UnassignPawn();
        }
        
        public void UnassignPawn()
        {
            AssignedPawn = null;
        }

        public bool AssignedAnything(Pawn pawn)
        {
            return AssignedPawn == pawn;
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (Gizmo g in base.GetGizmos())
            {
                yield return g;
            }
            yield return new Command_Action
            {
                defaultLabel = "CommandBedSetOwnerLabel".Translate(),
                icon = ContentFinder<Texture2D>.Get("UI/Commands/AssignOwner", true),
                defaultDesc = "CommandBedSetOwnerDesc".Translate(),
                action = delegate()
                {
                    Find.WindowStack.Add(new Dialog_AssignBuildingOwner(this));
                },
                hotKey = KeyBindingDefOf.Misc3
            };
            foreach (Gizmo g2 in StorageSettingsClipboard.CopyPasteGizmosFor(Settings))
            {
                yield return g2;
            }
        }

        public override string GetInspectString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (Faction == Faction.OfPlayer)
            {
                var owner = AssignedPawn != null ? AssignedPawn.Label : "Nobody".Translate();
                stringBuilder.AppendLine("Owner".Translate() + ": " + owner);    
            }
            return stringBuilder.ToString().TrimEndNewlines();
        }
    }
}