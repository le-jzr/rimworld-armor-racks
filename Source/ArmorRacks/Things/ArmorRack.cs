using UnityEngine;
using Verse;

namespace ArmorRacks.Things
{
    [StaticConstructorOnStartup]
    public class ArmorRack : ArmorRackBase
    {
        static Graphic_Multi rackGraphic;
        static Graphic_Multi rackHeadGraphic;

        protected override Graphic_Multi RackGraphic
        {
            get
            {
                if (rackGraphic == null)
                    rackGraphic = (Graphic_Multi)GraphicDatabase.Get<Graphic_Multi>("ArmorRacks_ArmorRack", ShaderDatabase.Cutout, new Vector2(1f, 1f), Color.white);

                return rackGraphic;
            }
        }

        protected override Graphic_Multi RackHeadGraphic
        {
            get
            {
                if (rackHeadGraphic == null)
                    rackHeadGraphic = (Graphic_Multi)GraphicDatabase.Get<Graphic_Multi>("ArmorRacks_ArmorRackHead", ShaderDatabase.Cutout, new Vector2(1f, 1f), Color.white);

                return rackHeadGraphic;
            }
        }
    }
}
