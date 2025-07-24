using UnityEngine;
using Verse;

namespace ArmorRacksSimplified
{
    public class ArmorRacksSimplifiedModSettings : ModSettings
    {
        public bool EquipSetForcedDefault = false;
        public bool TransferSetForcedDefault = false;
        public int EquipSpeedFactorDefault = 70;

        public bool EquipSetForced = false;
        public bool TransferSetForced = false;
        public int EquipSpeedFactor = 70;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref EquipSetForced, "EquipSetForced");
            Scribe_Values.Look(ref TransferSetForced, "TransferSetForced");
            Scribe_Values.Look(ref EquipSpeedFactor, "EquipSpeedFactor");
        }
    }

    public class ArmorRacksSimplifiedMod : Verse.Mod
    {
        public ArmorRacksSimplifiedModSettings Settings;

        public ArmorRacksSimplifiedMod(ModContentPack content) : base(content)
        {
            Settings = GetSettings<ArmorRacksSimplifiedModSettings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);
            listingStandard.CheckboxLabeled("ArmorRacks_EquipSetForcedModSetting_Label".Translate(), ref Settings.EquipSetForced);
            listingStandard.CheckboxLabeled("ArmorRacks_TransferSetForcedModSetting_Label".Translate(), ref Settings.TransferSetForced);
            listingStandard.GapLine();

            listingStandard.Label(
                "ArmorRacks_EquipSpeedFactorUnpowered_Label".Translate(),
                -1f,
                "ArmorRacks_EquipSpeedFactor_Tooltip".Translate());
            var buffer2 = Settings.EquipSpeedFactor.ToString();
            listingStandard.TextFieldNumeric(ref Settings.EquipSpeedFactor, ref buffer2);
            listingStandard.GapLine();
            if (listingStandard.ButtonText("ArmorRacks_RestoreDefaultSettings_Label".Translate()))
            {
                Settings.EquipSetForced = Settings.EquipSetForcedDefault;
                Settings.TransferSetForced = Settings.TransferSetForcedDefault;
                Settings.EquipSpeedFactor = Settings.EquipSpeedFactorDefault;
            }
            listingStandard.End();
            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "ArmorRacks_ModName".Translate();
        }
    }
}