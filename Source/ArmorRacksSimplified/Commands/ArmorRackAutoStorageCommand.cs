using Verse;
using ArmorRacksSimplified.Things;
using UnityEngine;

namespace ArmorRacksSimplified.Commands
{
    public class ArmorRackAutoStorageCommand : Command_Toggle
    {
        private readonly ArmorRackBase armorRack;

        public ArmorRackAutoStorageCommand(ArmorRackBase armorRack)
        {
            this.armorRack = armorRack;
            icon = ContentFinder<Texture2D>.Get(armorRack.def.graphicData.texPath + "_south", false);
            defaultIconColor = armorRack.Stuff.stuffProps.color;
            defaultLabel = "ArmorRacksSimplified_AutoStorageCommand_Label".Translate();
            defaultDesc = "ArmorRacksSimplified_AutoStorageCommand_Desc".Translate();
            toggleAction = delegate { ArmorRack.AutoSetStorageOnTransfer = !ArmorRack.AutoSetStorageOnTransfer; };
            isActive = () => ArmorRack.AutoSetStorageOnTransfer;
        }
    }
}
