using UnityEngine;
using Verse;

namespace ArmorRacks.Things
{
    [StaticConstructorOnStartup]
    public class MechanizedArmorRack : ArmorRackBase
    {
        static Graphic_Multi rackGraphic;

        protected override Graphic_Multi RackGraphic
        {
            get
            {
                if (rackGraphic == null)
                    rackGraphic = (Graphic_Multi)GraphicDatabase.Get<Graphic_Multi>("ArmorRacks_MechanizedArmorRack", ShaderDatabase.Cutout, new Vector2(1f, 2f), Color.white);

                return rackGraphic;
            }
        }

        protected override Graphic_Multi RackHeadGraphic => null;

        protected override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            var rot = flip ? Rotation.Opposite : Rotation;

            if (rot == Rot4.East)
                drawLoc.x += 0.4f;
            else if (rot == Rot4.West)
                drawLoc.x -= 0.4f;
            else if (rot == Rot4.North)
                drawLoc.z += 0.3f;

            base.DrawAt(drawLoc, flip);
        }
    }
}
