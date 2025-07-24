using RimWorld;
using UnityEngine;
using Verse;

namespace ArmorRacks.Things
{
    [StaticConstructorOnStartup]
    public class MendingArmorRack : ArmorRackBase
    {
        public int tickCounter = 0;
        private int? rareTicksPerMendCached;

        static Graphic_Multi rackGraphic;

        protected override Graphic_Multi RackGraphic
        {
            get
            {
                if (rackGraphic == null)
                    rackGraphic = (Graphic_Multi)GraphicDatabase.Get<Graphic_Multi>("ArmorRacks_MendingArmorRack", ShaderDatabase.Cutout, new Vector2(1f, 2f), Color.white);

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

        public int RareTicksPerMend
        {
            get
            {
                if (rareTicksPerMendCached == null)
                {
                    rareTicksPerMendCached = LoadedModManager.GetMod<ArmorRacksMod>().GetSettings<ArmorRacksModSettings>().RareTicksPerMend;
                }
                return (int)rareTicksPerMendCached;
            }
        }
        public override void TickRare()
        {
            base.TickRare();

            if (!GetComp<CompPowerTrader>().PowerOn)
                return;

            tickCounter++;
            if (tickCounter >= RareTicksPerMend)
            {
                MendContents();
                tickCounter = 0;
            }
        }

        public void MendContents()
        {
            foreach (var thing in this.HeldItems)
            {
                if (thing.HitPoints < thing.MaxHitPoints)
                {
                    thing.HitPoints++;
                }
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref tickCounter, "ArmorRackTickCounter");
        }
    }
}
