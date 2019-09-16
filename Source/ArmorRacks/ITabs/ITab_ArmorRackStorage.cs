using System.Collections.Generic;
using System.Linq;
using ArmorRacks.Things;
using RimWorld;
using UnityEngine;
using Verse;

namespace ArmorRacks.ITabs
{
    public class ITabArmorRackContents : ITab
    {
        private static readonly Vector2 WinSize = new Vector2(300f, 480f);
        private Vector2 scrollPosition;

        public ITabArmorRackContents()
        {
            this.size = WinSize;
            this.labelKey = "ArmorRacks_ArmorRackContentsTab_LabelKey";
            this.tutorTag = "ArmorRack";
        }

        protected override void FillTab()
        {
            Text.Font = GameFont.Small;
            Rect rect = new Rect(0.0f, 20f, this.size.x, this.size.y - 20f).ContractedBy(10f);
            Rect position = new Rect(rect.x, rect.y, rect.width, rect.height);
            GUI.BeginGroup(position);
            Text.Font = GameFont.Small;
            GUI.color = Color.white;
            Rect outRect = new Rect(0.0f, 0.0f, position.width, position.height);
            Rect viewRect = outRect;
            Widgets.BeginScrollView(outRect, ref this.scrollPosition, viewRect, true);
            float num = 0.0f;
            ArmorRack armorRack = SelThing as ArmorRack;
            
            Widgets.ListSeparator(ref num, viewRect.width, "Equipment".Translate());
            Thing storedWeapon = armorRack.GetStoredWeapon();
            if (storedWeapon != null)
            {
                this.DrawThingRow(ref num, viewRect.width, storedWeapon, false);
            }

            Widgets.ListSeparator(ref num, viewRect.width, "Apparel".Translate());
            foreach (Apparel apparel in armorRack.GetStoredApparel()
                .OrderByDescending((ap => ap.def.apparel.bodyPartGroups[0].listOrder)))
                this.DrawThingRow(ref num, viewRect.width, (Thing) apparel, false);

            Widgets.EndScrollView();
            GUI.EndGroup();
            GUI.color = Color.white;
            Text.Anchor = TextAnchor.UpperLeft;
        }

        private void DrawThingRow(ref float y, float width, Thing thing, bool inventory = false)
        {
            Rect rect1 = new Rect(0.0f, y, width, 28f);
            Widgets.InfoCardButton(rect1.width - 24f, y, thing);
            rect1.width -= 24f;
            Rect rect3 = rect1;
            rect3.xMin = rect3.xMax - 60f;
            rect1.width -= 60f;
            if (Mouse.IsOver(rect1))
            {
                GUI.color = ITab_Pawn_Gear.HighlightColor;
                GUI.DrawTexture(rect1, (Texture) TexUI.HighlightTex);
            }

            if ((Object) thing.def.DrawMatSingle != (Object) null &&
                (Object) thing.def.DrawMatSingle.mainTexture != (Object) null)
                Widgets.ThingIcon(new Rect(4f, y, 28f, 28f), thing, 1f);
            Text.Anchor = TextAnchor.MiddleLeft;
            GUI.color = ITab_Pawn_Gear.ThingLabelColor;
            Rect rect4 = new Rect(36f, y, rect1.width - 36f, rect1.height);
            string str1 = thing.LabelCap;
            Apparel ap = thing as Apparel;
            Text.WordWrap = false;
            Widgets.Label(rect4, str1.Truncate(rect4.width, (Dictionary<string, string>) null));
            Text.WordWrap = true;
            string str2 = thing.DescriptionDetailed;
            if (thing.def.useHitPoints)
                str2 = str2 + "\n" + (object) thing.HitPoints + " / " + (object) thing.MaxHitPoints;
            TooltipHandler.TipRegion(rect1, (TipSignal) str2);
            y += 28f;
        }
    }
}