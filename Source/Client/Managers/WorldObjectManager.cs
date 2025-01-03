using RimWorld.Planet;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace GameClient.SOS2RTCompat
{
    [RTManager]
    public static class WorldObjectManager
    {
        public static T RemoveWorldObjectFromTile<T>(int tile) 
        {
            List<WorldObject> toGet = Find.WorldObjects.AllWorldObjects.Where(x => x.Tile == tile).ToList();
            foreach (WorldObject obj in toGet)
            {
                if (toGet != null && toGet is T match)
                {
                    Find.WorldObjects.Remove(obj);
                }
            }
            return default;
        }

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
    }
}
