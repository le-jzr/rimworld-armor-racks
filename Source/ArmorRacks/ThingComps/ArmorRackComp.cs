using System;
using System.Collections.Generic;
using ArmorRacks.Things;
using RimWorld;
using Verse;

namespace ArmorRacks.ThingComps
{
    public class ArmorRackCompProperties : CompProperties
    {
        public bool myExampleBool;
        
        public ArmorRackCompProperties()
        {
            this.compClass = typeof(ArmorRackComp);
        }

        public ArmorRackCompProperties(Type compClass) : base(compClass)
        {
            this.compClass = compClass;
        }
    }
    
    public class ArmorRackComp : ThingComp
    {
        public ArmorRackCompProperties Props => (ArmorRackCompProperties)this.props;
        public bool ExampleBool => Props.myExampleBool;

        public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
        {
            ArmorRack armorRack = this.parent as ArmorRack;
            yield return new FloatMenuOption("Clear out", delegate
            {
                armorRack.DropContents();
            });
        }
    }
}