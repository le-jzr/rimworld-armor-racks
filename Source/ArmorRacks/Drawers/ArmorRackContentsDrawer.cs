using System.Collections.Generic;
using System.Linq;
using ArmorRacks.Things;
using RimWorld;
using UnityEngine;
using Verse;

namespace ArmorRacks.Drawers
{
    public class ArmorRackContentsDrawer
    {
        public readonly ArmorRack ArmorRack;
        public List<ApparelGraphicRecord> ApparelGraphics;

        public ArmorRackContentsDrawer(ArmorRack armorRack)
        {
            ArmorRack = armorRack;
            ApparelGraphics = new List<ApparelGraphicRecord>();
        }

        public Vector3 BaseHeadOffsetAt(Rot4 rotation)
        {
            Vector2 headOffset = ArmorRack.BodyTypeDef.headOffset;
            switch (rotation.AsInt)
            {
                case 0:
                    return new Vector3(0.0f, 0.0f, headOffset.y);
                case 1:
                    return new Vector3(headOffset.x, 0.0f, headOffset.y);
                case 2:
                    return new Vector3(0.0f, 0.0f, headOffset.y);
                case 3:
                    return new Vector3(-headOffset.x, 0.0f, headOffset.y);
                default:
                    return Vector3.zero;
            }
        }

        public void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            if (ApparelGraphics.Count != ArmorRack.GetStoredApparel().Count())
            {
                ResolveApparelGraphics();
            }
            
            const float angle = 0.0f;
            Quaternion quaternion = Quaternion.AngleAxis(angle, Vector3.up);
            Rot4 bodyFacing = ArmorRack.Rotation;
            Vector3 rootLoc = drawLoc;
            Vector3 vector3_1 = rootLoc;
            Vector3 vector3_2 = rootLoc;
            if (bodyFacing != Rot4.North)
            {
                vector3_2.y += 7f / 256f;
                vector3_1.y += 3f / 128f;
            }
            else
            {
                vector3_2.y += 3f / 128f;
                vector3_1.y += 7f / 256f;
            }

            Vector3 vector3_3 = quaternion * BaseHeadOffsetAt(bodyFacing);
            Vector3 loc1 = rootLoc + vector3_3;
            loc1.y += 1f / 32f;
            Mesh mesh = new GraphicMeshSet(1.5f).MeshAt(bodyFacing);

            for (int index = 0; index < ApparelGraphics.Count; ++index)
            {
                if (ApparelGraphics[index].sourceApparel.def.apparel.LastLayer == ApparelLayerDefOf.Overhead)
                {
                    if (!ApparelGraphics[index].sourceApparel.def.apparel.hatRenderedFrontOfFace)
                    {
                        Material mat = ApparelGraphics[index].graphic.MatAt(bodyFacing);
                        GenDraw.DrawMeshNowOrLater(mesh, loc1, quaternion, mat, false);
                    }
                    else
                    {
                        Material mat = ApparelGraphics[index].graphic.MatAt(bodyFacing);
                        Vector3 loc2 = rootLoc + vector3_3;
                        loc2.y += !(bodyFacing == Rot4.North) ? 9f / 256f : 1f / 256f;
                        GenDraw.DrawMeshNowOrLater(mesh, loc2, quaternion, mat, false);
                    }
                }
                else
                {
                    Material mat = ApparelGraphics[index].graphic.MatAt(bodyFacing);
                    GenDraw.DrawMeshNowOrLater(mesh, vector3_1, quaternion, mat, false);
                }
            }

            foreach (Apparel apparel in ArmorRack.GetStoredApparel())
            {
                apparel.DrawWornExtras();
            }
        }

        public void ResolveApparelGraphics()
        {
            ApparelGraphics.Clear();
            foreach (Apparel apparel in ArmorRack.GetStoredApparel())
            {
                ApparelGraphicRecord rec;
                if (ApparelGraphicRecordGetter.TryGetGraphicApparel(apparel, ArmorRack.BodyTypeDef, out rec))
                    ApparelGraphics.Add(rec);
            }
        }
    }
}