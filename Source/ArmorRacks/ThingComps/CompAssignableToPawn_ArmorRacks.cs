﻿using System;
using System.Collections.Generic;
using System.Linq;
using ArmorRacks.Commands;
using ArmorRacks.DefOfs;
using ArmorRacks.Things;
using ArmorRacks.Utils;
using RimWorld;
using Verse;
using Verse.AI;

namespace ArmorRacks.ThingComps
{
    public class CompAssignableToPawn_ArmorRacks : CompAssignableToPawn
    {
        public new int MaxAssignedPawnsCount => 1;
        
        public override IEnumerable<Pawn> AssigningCandidates
        {
            get
            {
                Log.Warning("TEST");
                if (!this.parent.Spawned)
                    return Enumerable.Empty<Pawn>();
                return this.parent.Map.mapPawns.FreeColonists;
            }
        }

        public new IEnumerable<Pawn> AssignedPawns
        {
            get
            {
                Log.Warning("TEST");
                var p = (ArmorRack) this.parent;
                if (p.AssignedPawn != null)
                {
                    yield return p.AssignedPawn;
                }
            }
        }

        public override void TryAssignPawn(Pawn pawn)
        {
            Log.Warning("TEST");
            var racks = pawn.Map.listerBuildings.AllBuildingsColonistOfClass<ArmorRack>();
            foreach (var rack in racks)
            {
                if (rack.AssignedPawn == pawn)
                {
                    rack.UnassignPawn();
                }
            }
            var p = (ArmorRack) this.parent;
            p.AssignedPawn = pawn;
        }

        public void TryUnassignPawn(Pawn pawn)
        {
            Log.Warning("TEST");
            UnassignPawn();
        }
        
        public void UnassignPawn()
        {
            Log.Warning("TEST");
            var p = (ArmorRack) this.parent;
            p.AssignedPawn = null;
        }

        public override bool AssignedAnything(Pawn pawn)
        {
            Log.Warning("TEST");
            var racks = pawn.Map.listerBuildings.AllBuildingsColonistOfClass<ArmorRack>();
            foreach (var rack in racks)
            {
                if (rack.AssignedPawn == pawn)
                {
                    return true;
                }
            }
            return false;
        }
        
    }

}