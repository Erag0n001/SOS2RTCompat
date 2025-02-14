﻿using RimWorld.Planet;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Tilemaps;
using Verse;

namespace GameClient.SOS2RTCompat
{
    [RTManager]
    public static class SOS2WorldObjectManager
    {
        public static T? FindWorldObjectFromTile<T>(int tile)
        {
            List<WorldObject> toGet = Find.WorldObjects.AllWorldObjects.Where(x => x.Tile == tile).ToList();
            foreach (WorldObject obj in toGet)
            {
                if (obj is T match)
                    return match;
            }
            return default;
        }

        public static WorldObjectFakeOrbitingShip FindShipFromID(int id) 
        {
            WorldObjectFakeOrbitingShip[] toGet =
                Find.WorldObjects.AllWorldObjects.Where(x => x is WorldObjectFakeOrbitingShip).
                Cast<WorldObjectFakeOrbitingShip>().ToArray();
            foreach(WorldObjectFakeOrbitingShip ship in toGet) 
            {
                if(id == ship.serverId)
                    return ship;
            }
            return null;
        }
    }
}
