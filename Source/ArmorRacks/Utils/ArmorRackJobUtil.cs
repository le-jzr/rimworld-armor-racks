using ArmorRacks.Things;
using RimWorld;
using Verse;

namespace ArmorRacks.Utils
{
    public static class ArmorRackJobUtil
    {
        public static bool RackHasItems(ArmorRack rack)
        {
            return rack.InnerContainer.Count != 0;
        }

        public static bool PawnCanEquipWeaponSet(ArmorRack rack, Pawn pawn)
        {
            return !(pawn.story.WorkTagIsDisabled(WorkTags.Violent) && rack.GetStoredWeapon() != null);
        }
    }
}