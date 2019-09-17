using System.Collections.Generic;
using System.Linq;
using ArmorRacks.Things;
using RimWorld;
using UnityEngine;
using Verse;
using System;

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

        public void DrawAt(Vector3 drawLoc)
        {
            if (ApparelGraphics.Count == 0)
            {
                ResolveApparelGraphics();
            }
            DrawApparel(drawLoc);
            DrawWeapon(drawLoc);
        }

        public void DrawApparel(Vector3 drawLoc)
        {
            const float angle = 0.0f;
            Quaternion quaternion = Quaternion.AngleAxis(angle, Vector3.up);
            Vector3 vector3_1 = drawLoc;
            Vector3 vector3_2 = drawLoc;
            if (ArmorRack.Rotation != Rot4.North)
            {
                vector3_2.y += 7f / 256f;
                vector3_1.y += 3f / 128f;
            }
            else
            {
                vector3_2.y += 3f / 128f;
                vector3_1.y += 7f / 256f;
            }

            Vector3 vector3_3 = quaternion * BaseHeadOffsetAt(ArmorRack.Rotation);
            Vector3 loc1 = drawLoc + vector3_3;
            loc1.y += 1f / 32f;
            Mesh mesh = new GraphicMeshSet(1.5f).MeshAt(ArmorRack.Rotation);

            for (int index = 0; index < ApparelGraphics.Count; ++index)
            {
                if (ApparelGraphics[index].sourceApparel.def.apparel.LastLayer == ApparelLayerDefOf.Overhead)
                {
                    if (!ApparelGraphics[index].sourceApparel.def.apparel.hatRenderedFrontOfFace)
                    {
                        Material mat = ApparelGraphics[index].graphic.MatAt(ArmorRack.Rotation);
                        GenDraw.DrawMeshNowOrLater(mesh, loc1, quaternion, mat, false);
                    }
                    else
                    {
                        Material mat = ApparelGraphics[index].graphic.MatAt(ArmorRack.Rotation);
                        Vector3 loc2 = drawLoc + vector3_3;
                        loc2.y += !(ArmorRack.Rotation == Rot4.North) ? 9f / 256f : 1f / 256f;
                        GenDraw.DrawMeshNowOrLater(mesh, loc2, quaternion, mat, false);
                    }
                }
                else
                {
                    Material mat = ApparelGraphics[index].graphic.MatAt(ArmorRack.Rotation);
                    GenDraw.DrawMeshNowOrLater(mesh, vector3_1, quaternion, mat, false);
                }
            }

            foreach (Apparel apparel in ArmorRack.GetStoredApparel())
            {
                apparel.DrawWornExtras();
            }
        }

        public void DrawWeapon(Vector3 drawLoc)
        {
            Thing storedWeapon = ArmorRack.GetStoredWeapon();
            if (storedWeapon == null)
            {
                return;
            }

            Vector3 weaponDrawLoc = drawLoc;
            Mesh weaponMesh;
            float angle = -90f;
            if (ArmorRack.Rotation == Rot4.South)
            {
                weaponDrawLoc += new Vector3(0.0f, 0.0f, 0.0f);
                weaponDrawLoc.y += 5f / 128f;
                weaponMesh = MeshPool.plane10;
                angle = -45f;
            }
            else if (ArmorRack.Rotation == Rot4.North)
            {
                weaponDrawLoc += new Vector3(0.0f, 0.0f, -0.11f);
                ref Vector3 local = ref weaponDrawLoc;
                weaponMesh = MeshPool.plane10;
            }
            else if (ArmorRack.Rotation == Rot4.East)
            {
                weaponDrawLoc += new Vector3(0.3f, 0.0f, -0.22f);
                weaponDrawLoc.y += 5f / 128f;
                weaponMesh = MeshPool.plane10;
            }
            else
            {
                weaponDrawLoc += new Vector3(-0.3f, 0.0f, -0.22f);
                weaponDrawLoc.y += 5f / 128f;
                weaponMesh = MeshPool.plane10Flip;
                angle = 90f;
            }

            Graphic_StackCount graphic = storedWeapon.Graphic as Graphic_StackCount;
            Material material = graphic == null
                ? storedWeapon.Graphic.MatSingle
                : graphic.SubGraphicForStackCount(1, storedWeapon.def).MatSingle;
            Graphics.DrawMesh(weaponMesh, weaponDrawLoc, Quaternion.AngleAxis(angle, Vector3.up), material, 0);
        }

        public void ResolveApparelGraphics()
        {
            Log.Warning("resolving");
            ApparelGraphics.Clear();
            var apparelList = ArmorRack.GetStoredApparel().ToList();
            apparelList.Sort(((a, b) => a.def.apparel.LastLayer.drawOrder.CompareTo(b.def.apparel.LastLayer.drawOrder)));
            foreach (Apparel apparel in apparelList)
            {
                ApparelGraphicRecord rec;
                if (ApparelGraphicRecordGetter.TryGetGraphicApparel(apparel, ArmorRack.BodyTypeDef, out rec))
                    ApparelGraphics.Add(rec);
            }
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
    }
}