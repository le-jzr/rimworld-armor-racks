using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using ArmorRacks.Things;
using RimWorld;
using Verse;

namespace ArmorRacks.Utils
{
    [StaticConstructorOnStartup]
    public static class ArmorRackJobUtil
    {
        private static MethodInfo CanWearMethodInfo;
        private static MethodInfo CanEquipMethodInfo;
        static ArmorRackJobUtil()
        {
            var runningMods = LoadedModManager.RunningModsListForReading;
            foreach (var runningMod in runningMods)
            {
                foreach (Assembly asm in runningMod.assemblies.loadedAssemblies)
                {
                    Type cls = asm.GetType("AlienRace.RaceRestrictionSettings");
                    if (cls != null)
                    {
                        CanWearMethodInfo = cls.GetMethod("CanWear");
                        CanEquipMethodInfo = cls.GetMethod("CanEquip");
                    }
                }
            }
        }

        public static bool RackHasItems(ArmorRack rack)
        {
            return rack.InnerContainer.Count != 0;
        }

        public static bool PawnCanEquipWeaponSet(ArmorRack rack, Pawn pawn)
        {
            return !(pawn.story.WorkTagIsDisabled(WorkTags.Violent) && rack.GetStoredWeapon() != null);
        }

        public static bool RaceCanWear(ThingDef apparel, ThingDef race)
        {
            if (CanWearMethodInfo != null)
            {
                var result = CanWearMethodInfo.Invoke(null, new [] {apparel, race});
                return (bool) result;
            }
            return true;
        }
        
        public static bool RaceCanEquip(ThingDef weapon, ThingDef race)
        {
            if (CanEquipMethodInfo != null)
            {
                var result = CanEquipMethodInfo.Invoke(null, new [] {weapon, race});
                return (bool) result;
            }
            return true;
        }
        
    }
}