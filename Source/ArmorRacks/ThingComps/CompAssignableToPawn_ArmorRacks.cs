using ArmorRacks.Things;
using RimWorld;
using Verse;

namespace ArmorRacks.ThingComps
{
    public class CompAssignableToPawn_ArmorRacks : CompAssignableToPawn
    {
        public override void TryAssignPawn(Pawn pawn)
        {
            var comp = pawn.GetComp<ArmorRackUseCommandComp>();
            comp.assignedRack?.GetComp<CompAssignableToPawn_ArmorRacks>().TryUnassignPawn(pawn);

            comp.assignedRack = (ArmorRackBase)this.parent;
            base.TryAssignPawn(pawn);
            comp.assignedRack.OwnerChanged();
        }

        public override void TryUnassignPawn(Pawn pawn, bool sort = true, bool uninstall = false)
        {
            base.TryUnassignPawn(pawn, sort, uninstall);
            var comp = pawn.GetComp<ArmorRackUseCommandComp>();
            comp.assignedRack = null;
            ((ArmorRackBase)this.parent).OwnerChanged();
        }

        public override bool AssignedAnything(Pawn pawn)
        {
            var comp = pawn.GetComp<ArmorRackUseCommandComp>();
            return comp.assignedRack != null;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();

            if (Scribe.mode == LoadSaveMode.PostLoadInit && assignedPawns.RemoveAll(x => x.GetComp<ArmorRackUseCommandComp>().assignedRack != parent) > 0)
                Log.Warning(parent.ToStringSafe() + " had pawns assigned that don't have it as an assigned armor rack. Removing.");
        }
    }
}
