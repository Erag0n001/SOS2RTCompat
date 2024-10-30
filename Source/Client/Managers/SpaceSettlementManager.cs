using GameClient;
using RimWorld;
using RimWorld.Planet;
using SaveOurShip2;
using Shared;
using Shared.SOS2RTCompat;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using static Shared.CommonEnumerators;
using CommonValues = Shared.SOS2RTCompat.CommonValues;

namespace GameClient.SOS2RTCompat
{
    public static class SpaceSettlementManager
    {
        public static List<WorldObjectFakeOrbitingShip> spacePlayerSettlement = new List<WorldObjectFakeOrbitingShip>();
            
        public static void ParsePacket(Packet packet) 
        {
            SpaceSettlementData spaceSettlement = Serializer.ConvertBytesToObject<SpaceSettlementData>(packet.contents);
            switch (spaceSettlement._stepMode)
            {
                case SettlementStepMode.Add:
                    SpawnSingleSettlement(spaceSettlement); 
                    break;
                case SettlementStepMode.Remove:
                    WorldObjectManager.RemoveWorldObjectFromTile<WorldObjectFakeOrbitingShip>(spaceSettlement._settlementData.Tile);
                    break;
            }
        }

        public static void ClearAllSettlements()
        {
            spacePlayerSettlement.Clear();

            WorldObject[] ships = Find.WorldObjects.AllWorldObjects.Where(worldObject => worldObject.def.defName == "RT_Ship " || worldObject.def.defName == "RT_ShipEnemy" || worldObject.def.defName == "RT_ShipNeutral").ToArray();
            foreach (WorldObject ship in ships) Find.WorldObjects.Remove(ship);
        }

        public static void SpawnSingleSettlement(SpaceSettlementData data)
        {
            try
            {
                WorldObjectFakeOrbitingShip ship = SetGoodWillShip(data._settlementData.Goodwill);
                ship.Tile = data._settlementData.Tile;
                ship.name = $"{data._settlementData.Owner}'s ship";
                ship.SetFaction(PlanetManagerHelper.GetPlayerFactionFromGoodwill(data._settlementData.Goodwill));
                ship.phi = data._phi;
                ship.theta = data._theta;
                ship.radius = data._radius;
                ship.OrbitSet();

                ship.altitude = 1000;
                Logger.Warning(StringUtilities.ToString(data));
                Logger.Warning(ship.ToString());
                spacePlayerSettlement.Add(ship);
                Find.WorldObjects.Add(ship);
            }
            catch (Exception e) 
            { 
                Logger.Error($"[SOS2]Failed to spawn ship at {data._settlementData.Tile}. Reason: {e}");
                Logger.Error($"[SOS2]Debbuging info:\n{StringUtilities.ToString(data)}");
            }
        }

        public static void ChangeGoodwill(int tile, Goodwill goodwill, WorldObjectFakeOrbitingShip oldship = null)
        {
            if (oldship == null)
            {
                oldship = WorldObjectManager.FindWorldObjectFromTile<WorldObjectFakeOrbitingShip>(tile);
            }
            spacePlayerSettlement.Remove(oldship);
            Find.WorldObjects.Remove(oldship);

            WorldObjectFakeOrbitingShip ship = SetGoodWillShip(goodwill);
            ship.Tile = oldship.Tile;
            ship.name = $"{oldship.name}";
            ship.phi = oldship.phi;
            ship.theta = oldship.theta;
            ship.radius = oldship.radius;
            ship.OrbitSet();
            ship.altitude = 1000;

            spacePlayerSettlement.Add(ship);
            Find.WorldObjects.Add(ship);
        }

        public static WorldObjectFakeOrbitingShip SetGoodWillShip(Goodwill goodwill)
        {
            WorldObjectFakeOrbitingShip ship;
            Logger.Warning("test2");
            switch (goodwill)
            {
                default:
                    Logger.Warning("N");
                    ship = (WorldObjectFakeOrbitingShip)WorldObjectMaker.MakeWorldObject(ShipDefOf.RT_ShipNeutral);
                    break;
                case Goodwill.Enemy:
                    Logger.Warning("E");
                    ship = (WorldObjectFakeOrbitingShip)WorldObjectMaker.MakeWorldObject(ShipDefOf.RT_ShipEnemy);
                    break;
                case Goodwill.Ally:
                    Logger.Warning("A");
                    ship = (WorldObjectFakeOrbitingShip)WorldObjectMaker.MakeWorldObject(ShipDefOf.RT_Ship);
                    break;
                case Goodwill.Faction:
                    Logger.Warning("F");
                    ship = (WorldObjectFakeOrbitingShip)WorldObjectMaker.MakeWorldObject(ShipDefOf.RT_Ship);
                    break;
            }
            Logger.Warning("test3");
            ship.SetFaction(PlanetManagerHelper.GetPlayerFactionFromGoodwill(goodwill));
            Logger.Warning("test4");
            return ship;
        }
    }
    public static class SpaceSettlementManagerHelper
    {
        public static void SendSettlementToServer(Map map)
        {
            ShipMapComp comp = map.GetComponent<ShipMapComp>();
            WorldObjectOrbitingShip orbitShip = comp.mapParent;
            SpaceSettlementData spaceSiteData = new SpaceSettlementData();

            spaceSiteData._settlementData.Tile = map.Tile;
            spaceSiteData._stepMode = SettlementStepMode.Add;
            spaceSiteData._theta = orbitShip.Theta;
            spaceSiteData._radius = orbitShip.Radius;
            orbitShip.Phi = UnityEngine.Random.Range(-70f, 70f);
            spaceSiteData._phi = orbitShip.Phi;
            Packet packet = Packet.CreateModdedPacketFromObject(nameof(SpaceSettlementManager), CommonValues.AssName, spaceSiteData);
            Network.listener.EnqueuePacket(packet);

            SaveManager.ForceSave();
        }
    }
}
